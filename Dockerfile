# Use a .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["HxStudioAuthService.csproj", "."]
RUN dotnet restore "./HxStudioAuthService.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet build "./HxStudioAuthService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "./HxStudioAuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use a .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "HxStudioAuthService.dll"]