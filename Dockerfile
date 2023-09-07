FROM mcr.microsoft.com/dotnet/runtime:8.0-preview AS base
WORKDIR /app

USER app
FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
ARG configuration=Release
WORKDIR /src
COPY ["RinhaInterpreterApp.csproj", "./"]
RUN dotnet restore "RinhaInterpreterApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RinhaInterpreterApp.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "RinhaInterpreterApp.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RinhaInterpreterApp.dll"]
