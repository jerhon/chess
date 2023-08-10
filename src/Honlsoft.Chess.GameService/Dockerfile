FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Honlsoft.Chess.GameService/Honlsoft.Chess.GameService.csproj", "Honlsoft.Chess.GameService/"]
RUN dotnet restore "Honlsoft.Chess.GameService/Honlsoft.Chess.GameService.csproj"
COPY . .
WORKDIR "/src/Honlsoft.Chess.GameService"
RUN dotnet build "Honlsoft.Chess.GameService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Honlsoft.Chess.GameService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Honlsoft.Chess.GameService.dll"]
