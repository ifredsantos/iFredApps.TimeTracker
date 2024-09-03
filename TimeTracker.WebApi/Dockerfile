#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../TimeTracker.WebApi/TimeTracker.WebApi.csproj", "TimeTracker.WebApi/"]
COPY ["../TimeTracker.Core/TimeTracker.Core.csproj", "TimeTracker.Core/"]
COPY ["../TimeTracker.Data/TimeTracker.Data.csproj", "TimeTracker.Data/"]
RUN dotnet restore "./TimeTracker.WebApi/TimeTracker.WebApi.csproj"
COPY . .
WORKDIR "/src/TimeTracker.WebApi"
RUN dotnet build "./TimeTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TimeTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]