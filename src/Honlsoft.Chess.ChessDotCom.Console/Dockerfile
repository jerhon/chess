FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Honlsoft.Chess.ChessDotCom.Console/Honlsoft.Chess.ChessDotCom.Console.csproj", "Honlsoft.Chess.ChessDotCom.Console/"]
RUN dotnet restore "Honlsoft.Chess.ChessDotCom.Console/Honlsoft.Chess.ChessDotCom.Console.csproj"
COPY . .
WORKDIR "/src/Honlsoft.Chess.ChessDotCom.Console"
RUN dotnet build "Honlsoft.Chess.ChessDotCom.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Honlsoft.Chess.ChessDotCom.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Honlsoft.Chess.ChessDotCom.Console.dll"]
