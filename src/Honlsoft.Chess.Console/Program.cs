// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Honlsoft.Chess.Engine;
using Spectre.Console;

var gameFactory = new ChessGameFactory();
var game = gameFactory.CreateStandardGame();

var randomGameEngine = new RandomEngine(game);

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
    AnsiConsole.WriteLine(game.CurrentPlayer + " to move.");
    
    if (game.CurrentPlayer == PieceColor.White) {
        var from = AnsiConsole.Console.AskPickAPiece();
        var (to, promotionPiece) = AnsiConsole.Console.AskMoveTo();
        lastMoveResult = game.MovePiece(from, to, promotionPiece);
    } else {
        var move = await randomGameEngine.SuggestMoveAsync(CancellationToken.None);
        var engineError = game.MovePiece(move.From, move.To, null);
    }
}

AnsiConsole.WriteLine("Game is over... " + game.GameState);
