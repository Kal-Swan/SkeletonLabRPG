# SkeletonLabRPG

## How it works
Login to system
Create a new RPG system and upload a RPG system PDF file to azure blob storage, saved against the user.
Select your RPG system and ask a question on the type of RPG build you want e.g. "I want a defensive build"
The question is sent to the .Net Api, stored in Cosmos DB and Queued to Azure Bus Service
Python App is listening to queue in Bus service, picks up question with user Id, based on this information gets the appropriate file from blob storage stored against the user Id.
Gets the contents of the file, cleans it for learning and sends it to Open AI to learn and build the answer.
Sends answer to .Net API
.Net API stores answer in Cosmos DB and uses Signal R to immediately send answer to Svelte app.
Also, using SignalR for progress bar so that user can see how long it takes.

# Architecture

Svelte → .NET API → Cosmos DB / Queue (Azure Bus Service) → Python service → Blob Storage → OpenAI


# Tech Stack

- Svelte
- .Net C#
- Python
- Open AI
- Cosmos DB
- Blob Storage
- Azure Container Apps
- SignalR




