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
from .build_model import LLMBuilds
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

parser = PydanticOutputParser(pydantic_object=LLMBuilds)
prompt_template = ChatPromptTemplate.from_template(instruction).partial(format_instructions=parser.get_format_instructions())

_faiss_db_cache = {}
container_name = "user-build-systems"

def get_blob_bytes(blob_name: str, blobServiceClient: BlobServiceClient):
    try:
        client = blobServiceClient.get_blob_client(container=container_name, blob=blob_name)
        stream = client.download_blob()
        return stream
    except Exception as e:
        raise ValueError(f"Failed to retrieve file from blob: {e}")

def get_blob_container_blobs(user_id: str, id: str, blobServiceClient: BlobServiceClient):
    try:
        client = blobServiceClient.get_container_client(container=container_name)
        print(f"{user_id}/{id}/")
        blobs = list(client.list_blobs(name_starts_with=f"{user_id}/{id}/"))
        return blobs
    except Exception as e:
        raise ValueError(f"Failed to retrieve file from blob: {e}")

async def process_data(build_system_id: str,question: str, rpg_system: str, user_id: str):
    if user_id not in _faiss_db_cache:
        if not blobEndpoint:
            raise Exception("blobEndpoint is empty, AZURE_BLOB_STORAGE_URL is not set in environment variables or something else went wrong.") 
        raw_documents=[]
        account_url = blobEndpoint
        service = BlobServiceClient(account_url=account_url, credential=DefaultAzureCredential())
        blobs = get_blob_container_blobs(user_id, build_system_id, service)
        for blob in blobs:
            stream = get_blob_bytes(blob.name, service)
            blob_bytes = stream.readall()
            content_type = stream.properties.content_settings.content_type
            if content_type == "text/plain":
                file_content = blob_bytes.decode("utf-8")
                url_list = [url.strip().replace("\r\n", "").replace('"', "") for url in file_content.split(",") if url.strip()]
                loader = WebBaseLoader(url_list)
                raw_documents = [doc async for doc in loader.alazy_load()]
            elif content_type == "application/pdf":
                pdf_file = io.BytesIO(blob_bytes)
                reader = PdfReader(pdf_file)
                docs = [
                    Document(page_content=page.extract_text())
                    for page in reader.pages
                ]
                raw_documents.extend(docs)

        if not raw_documents:
            return None

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
        _faiss_db_cache[user_id] = db
    else:
        db = _faiss_db_cache[user_id]

    # The K refers to chunks of meaningful data. Highers are better for pdfs while lower for web pages
    # But will use bg3 and daggerheart for now to decide on the chunk number
    # retriever=db.as_retriever(search_kwargs={"k": 5 if rpg_system == bg3 else 20}),
    chain = RetrievalQA.from_chain_type(
        llm=llm,
        retriever=db.as_retriever(search_kwargs={"k": 20 }),
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

