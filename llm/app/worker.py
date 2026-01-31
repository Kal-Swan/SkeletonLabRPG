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
                        print(f"Received message: {str(msg)}")
                        build_request = json.loads(str(msg))
                        user_id = build_request["user_id"]
                        build_request_id = build_request["build_request_id"]
                        build_system_id = build_request["build_system_id"]
                        rpg_system = build_request["build_system"].lower()
                        question = build_request["question"]
                        result = await process_data(build_system_id, question, rpg_system, user_id)

                        if result is None:
                            print(f"Error no blob found")
                            await receiver.complete_message(msg)
                            continue

                        print(f"Received result: {str(result)}")
                        headers = {
                            "X-Worker-Api-Key": worker_api_key,
                            "User-Id": user_id,
                            "Content-Type": "application/json"
                        }

                        print(f"api_url: {api_url}")
                        print(f"Notifying build request api with id: {build_request_id}")
                        print(f"Result data: {result.model_dump()}")
                        print(f"Headers: {headers}")

                        response = await http_client.post(
                            url=f"{api_url}/api/v1/buildrequest/notify/{build_request_id}",
                            json=result.model_dump(),
                            headers=headers)
                        
                        print(f"Notify response status: {response.status_code}, body: {response.text}")
                        
                        if 200 <= response.status_code < 300:
                            await receiver.complete_message(msg)
                            print(f"Message {build_request_id} completed")
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






