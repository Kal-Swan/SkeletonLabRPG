# Use the ASP.NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution and restore dependencies
COPY SkeletonLabRpg.Api/*.csproj SkeletonLabRpg.Api/
COPY SkeletonLabRpg.Common/*.csproj SkeletonLabRpg.Common/
COPY SkeletonLabRpg.Ml/*.csproj SkeletonLabRpg.Ml/
RUN dotnet restore SkeletonLabRpg.Api/SkeletonLabRpg.Api.csproj

# Copy all source code and build
COPY . .
WORKDIR /src/SkeletonLabRpg.Api
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SkeletonLabRpg.Api.dll"]