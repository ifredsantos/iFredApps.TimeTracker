# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8081
EXPOSE 443

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar e restaurar as dependências em uma única etapa para eficiência
COPY iFredApps.TimeTracker.WebApi/iFredApps.TimeTracker.WebApi.csproj iFredApps.TimeTracker.WebApi/
COPY iFredApps.TimeTracker.Core/iFredApps.TimeTracker.Core.csproj iFredApps.TimeTracker.Core/
COPY iFredApps.TimeTracker.Data/iFredApps.TimeTracker.Data.csproj iFredApps.TimeTracker.Data/
RUN dotnet restore iFredApps.TimeTracker.WebApi/iFredApps.TimeTracker.WebApi.csproj

# Copiar o restante do código-fonte
COPY . .

# Construir o projeto
WORKDIR /src/iFredApps.TimeTracker.WebApi
RUN dotnet build -c Release -o /app/build

# Etapa de publicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa final (runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "iFredApps.TimeTracker.WebApi.dll"]
