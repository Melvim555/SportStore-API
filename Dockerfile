# Use the official .NET 8.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["SportStore/SportStore.csproj", "SportStore/"]
RUN dotnet restore "SportStore/SportStore.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/SportStore"
RUN dotnet build "SportStore.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SportStore.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app

# Copy the published application from the publish stage
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "SportStore.dll"]