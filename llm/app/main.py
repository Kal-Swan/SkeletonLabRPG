from fastapi import FastAPI
from pydantic import BaseModel
from azure_llm_logic import process_data
# nest_asyncio is required to allow LangChain to run its own async functions
# within the event loop already managed by FastAPI/Uvicorn.
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