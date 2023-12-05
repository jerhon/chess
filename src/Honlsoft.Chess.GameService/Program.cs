using Honlsoft.Chess;
using Honlsoft.Chess.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ChessGameFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.MapPost("/game", (IMemoryCache cache, ChessGameFactory gameFactory) => {

        Guid gameId = Guid.NewGuid();
        ChessGame game = gameFactory.CreateStandardGame();
        FenSerializer fenSerializer = new FenSerializer();
        string fen = fenSerializer.Serialize(game.CurrentPosition);

        cache.Set(gameId, game);
        
        return new NewGame(gameId, PieceColor.White, fen);
    }).WithName("NewGame")
    .WithOpenApi();


app.MapGet("/game/{gameId}", (IMemoryCache cache, Guid gameId) => {
        if (cache.TryGetValue(gameId, out ChessGame game)) {
            FenSerializer fenSerializer = new FenSerializer();
            string fen = fenSerializer.Serialize(game.CurrentPosition);
            return Results.Ok(new Game(gameId, fen));
        } else {
            return Results.NotFound();
        }
    })
    .WithName("GetGame")
    .WithOpenApi();

app.MapPost("/game/{gameId}/move", (IMemoryCache cache, Guid gameId, [FromBody] GameMoveRequest move) => {
        if (cache.TryGetValue(gameId, out ChessGame game)) {

            SanSerializer serializer = new();
            San san = serializer.Deserialize(move.Move);

            // TODO: need to parse movetext and apply it...
            var result = game.Move(san);

            FenSerializer fenSerializer = new FenSerializer();
            string fen = fenSerializer.Serialize(game.CurrentPosition);
            return Results.Ok(new GameMoveResponse(result, fen));
        } else {
            return Results.NotFound();
        }
    })
    .WithName("SendMove")
    .WithOpenApi();


app.Run();



record NewGame(Guid GameId, PieceColor YourColor, string Fen) {
    
}

record Game(Guid GameId, string Fen) {
    
}

record GameMoveRequest(string Move) {
    
}

record GameMoveResponse(MoveResult Result, string Fen) {
    
}