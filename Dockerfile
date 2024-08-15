FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TimeTracker.WebApi/TimeTracker.WebApi.csproj", "TimeTracker.WebApi/"]
COPY ["TimeTracker.Core/TimeTracker.Core.csproj", "TimeTracker.Core/"]
COPY ["TimeTracker.Data/TimeTracker.Data.csproj", "TimeTracker.Data/"]

WORKDIR "/src/TimeTracker.WebApi"
RUN dotnet restore "TimeTracker.WebApi.csproj" --disable-parallel

COPY . .
RUN dotnet publish "TimeTracker.WebApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]
