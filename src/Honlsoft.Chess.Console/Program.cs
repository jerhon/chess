// See https://aka.ms/new-console-template for more information

using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Honlsoft.Chess.Engine;
using Spectre.Console;

var chessBoard = new ChessBoardBuilder().AddStandardGamePieces().Build();
var gameFactory = new ChessGameFactory();
var game = gameFactory.CreateGame(chessBoard);


var randomGameEngine = new RandomEngine(game);


// start playing
var playing = true;
MoveResult lastMoveResult = MoveResult.ValidMove;

while (playing) {
    
    AnsiConsole.Console.Clear();
    var view = new ChessBoardView(game.CurrentBoard);
    AnsiConsole.Write(view);
    if (lastMoveResult != MoveResult.ValidMove) {
        AnsiConsole.Write(new Paragraph(lastMoveResult + "\n", new Style(Color.White, Color.DarkRed)));
    }
    AnsiConsole.WriteLine(game.CurrentPlayer + " to move.");
    
    if (game.CurrentPlayer == PieceColor.White) {
        var from = AnsiConsole.Console.AskPickAPiece();
        var to = AnsiConsole.Console.AskMoveTo();
        lastMoveResult = game.MovePiece(from, to);
    } else {
        var move = await randomGameEngine.SuggestMoveAsync(CancellationToken.None);
        var engineError = game.MovePiece(move.From, move.To);
    }
}

