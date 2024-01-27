// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;
using Honlsoft.Chess.Uci.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Spectre.Console;

var gameFactory = new ChessGameFactory();
var game = gameFactory.CreateStandardGame();

//var randomGameEngine = new RandomEngine(game);


Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Honlsoft", LogEventLevel.Verbose)
    .WriteTo.File("C:\\logs\\honlsoft.chess.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

ServiceCollection collection = new ServiceCollection();
collection.AddLogging((builder) => {
    builder.ClearProviders();
    builder.AddSerilog();
});

var serviceProvider = collection.BuildServiceProvider();

var uciProcess = new UciProcess(@"C:\Jeremy\stockfish\16\stockfish-windows-x86-64-avx2.exe", serviceProvider);
uciProcess.Start();
var uciClient = new UciClient(uciProcess.Interface);

var uciChessEngine = new UciEngine(game, uciClient);

await uciChessEngine.InitializeEngineAsync(CancellationToken.None);

// start playing
MoveResult lastMoveResult = MoveResult.ValidMove;

// The move loop, keep moving until the game is over
while (game.GameState is ChessGameState.Check or ChessGameState.PlayerToMove) {
    
    AnsiConsole.Console.Clear();
    var view = new ChessBoardView(game.CurrentPosition);
    AnsiConsole.Write(view);
    if (lastMoveResult != MoveResult.ValidMove) {
        AnsiConsole.Write(new Paragraph(lastMoveResult + "\n", new Style(Color.White, Color.DarkRed)));
    }
    AnsiConsole.WriteLine(game.CurrentPosition.PlayerToMove + " to move.");
    
    if (game.CurrentPosition.PlayerToMove == PieceColor.White) {
        var from = AnsiConsole.Console.AskPickAPiece();
        var (to, promotionPiece) = AnsiConsole.Console.AskMoveTo();
        var san = SanMove.From(from, to);
        lastMoveResult = game.Move(san);
        if (lastMoveResult == MoveResult.ValidMove) {
            uciChessEngine.MakeMove(san);
            await uciChessEngine.UpdatePositionAsync(CancellationToken.None);
        }
    } else {
        var engineLines = await uciChessEngine.StartCalculatingAsync(CancellationToken.None);
        var bestMove = await uciChessEngine.StopCalculatingAsync(CancellationToken.None);
        
        var engineError = game.Move(bestMove.Move);
        uciChessEngine.MakeMove(bestMove.Move);
    }
}

AnsiConsole.WriteLine("Game is over... " + game.GameState);
