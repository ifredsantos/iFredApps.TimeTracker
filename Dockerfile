FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TimeTracker.WebApi/TimeTracker.WebApi.csproj", "TimeTracker.WebApi/"]
RUN dotnet restore "TimeTracker.WebApi/TimeTracker.WebApi.csproj"
COPY . .
WORKDIR "/src/TimeTracker.WebApi"
RUN dotnet build "TimeTracker.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTracker.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.WebApi.dll"]
