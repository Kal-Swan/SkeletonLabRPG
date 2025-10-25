import os
import time
import io
from dotenv import load_dotenv
from langchain_openai import AzureChatOpenAI, AzureOpenAIEmbeddings
from azure.identity import DefaultAzureCredential, get_bearer_token_provider
from langchain.document_loaders import WebBaseLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.vectorstores import FAISS
from langchain.prompts import ChatPromptTemplate
from langchain.chains import RetrievalQA
from langchain.output_parsers import PydanticOutputParser
from langchain.schema import Document
from openai import RateLimitError
from langchain.document_transformers import Html2TextTransformer
from .build_model import BuildList
from .prompt_instructions import instruction
from .constants import bg3, daggerheart
from azure.storage.blob import BlobServiceClient
from PyPDF2 import PdfReader
import requests

load_dotenv()

blobEndpoint = os.getenv("AZURE_BLOB_STORAGE_URL")
azureOpenAiEndpoint = os.getenv("AZURE_OPEN_AI_ENDPOINT")
azureAiTextEmbedding = os.getenv("AZURE_AI_TEXT_EMBEDDING")

token_provider = get_bearer_token_provider(
    DefaultAzureCredential(),
    "https://cognitiveservices.azure.com/.default"
)

llm = AzureChatOpenAI(
    azure_deployment="gpt-4.1",
    azure_endpoint=azureOpenAiEndpoint,
    api_version="2025-01-01-preview",
    azure_ad_token_provider=token_provider,
    temperature=1,
)

embeddings = AzureOpenAIEmbeddings(
    azure_deployment=azureAiTextEmbedding,
    azure_endpoint=azureOpenAiEndpoint,
    api_version="2024-12-01-preview",
    azure_ad_token_provider=token_provider,
)

parser = PydanticOutputParser(pydantic_object=BuildList)
prompt_template = ChatPromptTemplate.from_template(instruction).partial(format_instructions=parser.get_format_instructions())

_faiss_db_cache = {}

# test

async def get_blob_bytes(blob_name: str) -> bytes:
    try:
        if not blobEndpoint:
            raise Exception("blobEndpoint is empty, AZURE_BLOB_STORAGE_URL is not set in environment variables or something else went wrong.") 
        account_url = blobEndpoint
        container_name = "source-files"
        service = BlobServiceClient(account_url=account_url, credential=DefaultAzureCredential())
        client = service.get_blob_client(container=container_name, blob=blob_name)
        stream = client.download_blob()
        return stream.readall()
    except Exception as e:
        raise ValueError(f"Failed to retrieve file from blob: {e}")

def test_fetch():
    r = requests.get("https://bg3.wiki/wiki/Gale", headers={"User-Agent": "MyLlmApp/1.0"})
    print(r.status_code, len(r.text))


async def process_data(question: str, rpg_system: str):
    test_fetch()
    if rpg_system not in  _faiss_db_cache:
        raw_documents = []

        if rpg_system == bg3:
            blob_bytes = await get_blob_bytes(blob_name="bg3/bg3_urls.txt")
            file_content = blob_bytes.decode("utf-8")
            url_list = [url.strip().replace("\r\n", "").replace('"', "") for url in file_content.split(",") if url.strip()]
            loader = WebBaseLoader(url_list)
            raw_documents = [doc async for doc in loader.alazy_load()]
        elif rpg_system == daggerheart:
            blob_bytes = await get_blob_bytes(blob_name="daggerheart/streamlined_daggerheart_core_rulebook_2.pdf")
            pdf_file = io.BytesIO(blob_bytes)
            reader = PdfReader(pdf_file)
            docs = [
                Document(page_content=page.extract_text())
                for page in reader.pages
            ]
            raw_documents.extend(docs)
            
        cleaner = Html2TextTransformer()
        documents = cleaner.transform_documents(raw_documents)
        splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=100)
        chunks = splitter.split_documents(documents)

        batch_size = 25
        embedded_chunks = []
        batches: list[Document] = []
        for i in range(0, len(chunks), batch_size):
            batch = chunks[i:i+batch_size]
            batches.extend(batch)
            try:
                embeddings_batch = embeddings.embed_documents([doc.page_content for doc in batch])
                embedded_chunks.extend(embeddings_batch)
                time.sleep(1)
            except RateLimitError:
                time.sleep(60) 
        
        text_embeddings = [
            (doc.page_content, vector)
            for doc, vector in zip(batches, embedded_chunks)
        ]

        db = FAISS.from_embeddings(text_embeddings=text_embeddings, embedding=embeddings, metadatas=[doc.metadata for doc in batches])
        _faiss_db_cache[rpg_system] = db
    else:
        db = _faiss_db_cache[rpg_system]

    # The K refers to chunks of meaningful data. Highers are better for pdfs while lower for web pages
    # But will use bg3 and daggerheart for now to decide on the chunk number
    chain = RetrievalQA.from_chain_type(
        llm=llm,
        retriever=db.as_retriever(search_kwargs={"k": 5 if rpg_system == bg3 else 20}),
        chain_type="stuff",
        chain_type_kwargs={"prompt": prompt_template},
        input_key="question",
        return_source_documents=True
    )
    combined_question = f"You are an expert RPG strategist specializing in {rpg_system}.\n {question}"

    result = await chain.ainvoke({"question": combined_question})
    try:
        return parser.parse(result["result"])
    except Exception as e:
        raise ValueError(f"Failed to parse output as Build object: {e}\nRaw output:\n{result}")
