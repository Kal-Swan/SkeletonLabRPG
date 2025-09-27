from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
# import asyncio
from .sources import all_sources

from app.azure_llm_logic import process_data
# from rag_pipeline import generate_rpg_answer
import nest_asyncio
nest_asyncio.apply()
app = FastAPI()

class QuestionRequest(BaseModel):
    rpg_system: str
    question: str

@app.post("/api/llm/question")
async def ask_rpg_question(data: QuestionRequest):
    rpg_system = data.rpg_system.lower()
    # if rpg_system not in all_sources:
    #     raise HTTPException(status_code=404, detail=f"RPG system '{rpg_system}' not found.")
    result = await process_data(data.question, rpg_system)
    return result