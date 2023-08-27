FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Honlsoft.Chess.Console/Honlsoft.Chess.Console.csproj", "Honlsoft.Chess.Console/"]
RUN dotnet restore "Honlsoft.Chess.Console/Honlsoft.Chess.Console.csproj"
COPY . .
WORKDIR "/src/Honlsoft.Chess.Console"
RUN dotnet build "Honlsoft.Chess.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Honlsoft.Chess.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Honlsoft.Chess.Console.dll"]
