# Use a imagem oficial do .NET como imagem base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use a imagem oficial do SDK do .NET para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie os arquivos de projeto e restaure as dependências
COPY ["TimeTracker.WebApi/TimeTracker.WebApi.csproj", "TimeTracker.WebApi/"]
COPY ["TimeTracker.Core/TimeTracker.Core.csproj", "TimeTracker.Core/"]
COPY ["TimeTracker.Data/TimeTracker.Data.csproj", "TimeTracker.Data/"]
RUN dotnet restore "TimeTracker.WebApi/TimeTracker.WebApi.csproj" --disable-parallel

# Copie todo o código fonte e publique a aplicação
COPY . .
WORKDIR "/src/TimeTracker.WebApi"
RUN dotnet publish "TimeTracker.WebApi.csproj" -c Release -o /app/publish --no-restore

# Use a imagem base para rodar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]
