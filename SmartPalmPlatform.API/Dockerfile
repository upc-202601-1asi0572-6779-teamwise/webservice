# Step 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY SmartPalmPlatform.API/*.csproj SmartPalmPlatform.API/
RUN dotnet restore ./SmartPalmPlatform.API
COPY . .
RUN dotnet publish ./SmartPalmPlatform.API -c Release -o out

# Step 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartPalmPlatform.API.dll"]