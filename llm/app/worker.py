import asyncio
from azure.servicebus.aio import ServiceBusClient
from azure.identity.aio import DefaultAzureCredential as AsyncDefaultAzureCredential
from azure.identity import DefaultAzureCredential as SyncDefaultAzureCredential
from azure.appconfiguration.provider import (
    load,
    SettingSelector
)
import json
import httpx
import os

from .azure_llm_logic import process_data

credential = SyncDefaultAzureCredential()
azure_app_config_endpoint = os.getenv("AZURE_APP_CONFIGURATION_ENDPOINT")
env = os.getenv("LLM_ENV", "")
current_env = env if env.lower() else '\0'

if azure_app_config_endpoint is None or azure_app_config_endpoint.strip() == "":
    raise RuntimeError("Azure App Configuration endpoint not configured")

selects = [SettingSelector(key_filter="*", label_filter=current_env)]
config = load(endpoint=azure_app_config_endpoint, credential=credential, selects=selects)

servicebus_endpoint = config.get("ServiceBus:Endpoint")
queue_name = config.get("ServiceBus:QueueName")
api_url = config.get("ApiUrl")
worker_api_key = config.get("WorkerApi:Key")

async def main():
    async_credential = AsyncDefaultAzureCredential()
    async with ServiceBusClient(fully_qualified_namespace=servicebus_endpoint, credential=async_credential) as client:
        async with client.get_queue_receiver(queue_name=queue_name) as receiver:
            async with httpx.AsyncClient(timeout=30) as http_client:
                async for msg in receiver:
                    try:
                        build_request = json.loads(str(msg))
                        id = build_request["id"]
                        rpg_system = build_request["build_system"].lower()
                        question = build_request["question"]
                        result = await process_data(question, rpg_system)

                        headers = {
                            "X-Worker-Api-Key": worker_api_key,
                            "Content-Type": "application/json"
                        }

                        response = await http_client.post(
                            url=f"{api_url}/api/v1/buildrequest/notify/{id}",
                            json=result.model_dump(),
                            headers=headers)
                        
                        if 200 <= response.status_code < 300:
                            await receiver.complete_message(msg)
                            print(f"Message {id} completed")
                        else:
                            await receiver.abandon_message(msg)
                            print(f"Notify failed ({response.status_code}): {response.text}")
                    except Exception as e:
                        print(f"Error processing message: {e}")
                        await receiver.abandon_message(msg)


try:
    loop: None | asyncio.AbstractEventLoop = asyncio.get_running_loop()
except RuntimeError:
    loop = None

if loop and loop.is_running():
    # already inside a running loop, schedule as a task
    task = loop.create_task(main())
else:
    asyncio.run(main())






