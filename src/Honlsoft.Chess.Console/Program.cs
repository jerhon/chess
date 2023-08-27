// See https://aka.ms/new-console-template for more information

using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Spectre.Console;
using File = Honlsoft.Chess.File;

var chessBoard = new ChessBoardBuilder().AddStandardGamePieces().Build();
var gameFactory = new ChessGameFactory();
var game = gameFactory.CreateGame(chessBoard);

var playing = true;

while (playing) {
    AnsiConsole.Console.Clear();
    var view = new ChessBoardView(game.CurrentBoard);
    AnsiConsole.Write(view);
    var from = AnsiConsole.Console.AskPickAPiece();
    var to = AnsiConsole.Console.AskMoveTo();
    var (valid, reason) = game.MovePiece(from, to);
    if (!valid) {
        AnsiConsole.WriteLine("Move invalid. " + reason);
    }
}

