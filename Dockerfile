# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copie os arquivos de projeto e restaure as dependências
COPY ./TimeTracker.WebApi/TimeTracker.WebApi.csproj ./TimeTracker.WebApi/
COPY ./TimeTracker.Core/TimeTracker.Core.csproj ./TimeTracker.Core/
COPY ./TimeTracker.Data/TimeTracker.Data.csproj ./TimeTracker.Data/

RUN dotnet restore "./TimeTracker.WebApi/TimeTracker.WebApi.csproj" --disable-parallel

# Copie o restante do código e publique a aplicação
COPY . .
RUN dotnet publish "./TimeTracker.WebApi/TimeTracker.WebApi.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80

ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]
