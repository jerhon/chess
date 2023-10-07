// See https://aka.ms/new-console-template for more information

using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Uci.Client;
using Honlsoft.Chess.Uci.Engine;
using Spectre.Console;
using Spectre.Console.Rendering;
using File = Honlsoft.Chess.File;

var chessBoard = new ChessBoardBuilder().AddStandardGamePieces().Build();
var gameFactory = new ChessGameFactory();
var game = gameFactory.CreateGame(chessBoard);


var randomGameEngine = new RandomEngine(game);


// start playing
var playing = true;
string lastError = string.Empty;

while (playing) {
    
    AnsiConsole.Console.Clear();
    var view = new ChessBoardView(game.CurrentBoard);
    AnsiConsole.Write(view);
    if (!string.IsNullOrWhiteSpace(lastError)) {
        AnsiConsole.Write(new Paragraph(lastError + "\n", new Style(Color.White, Color.DarkRed)));
    }
    AnsiConsole.WriteLine(game.CurrentPlayer + " to move.");
    
    if (game.CurrentPlayer == PieceColor.White) {
        var from = AnsiConsole.Console.AskPickAPiece();
        var to = AnsiConsole.Console.AskMoveTo();
        (var success, lastError) = game.MovePiece(from, to);
    } else {
        var move = await randomGameEngine.SuggestMoveAsync(CancellationToken.None);
        var (engineSuccess, engineLastError) = game.MovePiece(move.From, move.To);
    }
}

