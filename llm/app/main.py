# from fastapi import FastAPI
# from pydantic import BaseModel
# from .azure_llm_logic import process_data
# import os
# # from fastapi.middleware.cors import CORSMiddleware
# # from azure.identity import DefaultAzureCredential
# # from azure.appconfiguration.provider import (
# #     load,
# #     SettingSelector
# # )
# # nest_asyncio is required to allow LangChain to run its own async functions
# # within the event loop already managed by FastAPI/Uvicorn.
# # import logging
# # import sys
# # logging.basicConfig(
# #     level="INFO",
# #     stream=sys.stdout
# # )
# # logger=logging.getLogger("skeletonlabrpg")

# # ENV = os.getenv("LLM_ENV","").lower()

# # azure_config_endpoint = os.getenv("AZURE_APP_CONFIGURATION_ENDPOINT")
# # credential = DefaultAzureCredential()
# # selects = {SettingSelector(key_filter="*", label_filter=ENV)}
# # config = load(endpoint=azure_config_endpoint, credential=credential, selects=selects)
# DEBUG = os.getenv("DEBUG", "true").lower() == "true"

# if DEBUG:
#     import nest_asyncio
#     nest_asyncio.apply()

# app = FastAPI()

# class QuestionRequest(BaseModel):
#     rpg_system: str
#     question: str

# @app.post("/api/llm/question")
# async def ask_rpg_question(data: QuestionRequest):
#     rpg_system = data.rpg_system.lower()
#     # if rpg_system not in all_sources:
#     #     raise HTTPException(status_code=404, detail=f"RPG system '{rpg_system}' not found.")
#     result = await process_data(data.question, rpg_system)
#     return result