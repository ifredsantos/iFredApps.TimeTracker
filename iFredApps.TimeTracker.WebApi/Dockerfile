FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything to the container
COPY . .

# Restore only the WebApi project to avoid referencing projects outside the build context
RUN dotnet restore "iFredApps.TimeTracker.WebApi/iFredApps.TimeTracker.WebApi.csproj"

# Publish webapi
WORKDIR /src/iFredApps.TimeTracker.WebApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "iFredApps.TimeTracker.WebApi.dll"]
