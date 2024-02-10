using Honlsoft.Chess.Serialization;
using Spectre.Console;

namespace Honlsoft.Chess.Console.UseCases;

public class ReplayGame {
    
    public async Task ReplayAsync(FileInfo pgnFile)
    {
        var pgn = await File.ReadAllTextAsync(pgnFile.FullName);
        var pgnGame = PgnSerializer.Default.Deserialize(pgn);
        var game = ChessGameFactory.Default.CreateStandardGame();
        var moves = pgnGame.Moves;
        var success = true;

        foreach (var move in moves) {
            var result = game.Move(move.Move);
            if (result != MoveResult.ValidMove) {
                AnsiConsole.WriteLine("FAILED WITH MOVE: {move}");
                break;
                
            }

            AnsiConsole.Write(new Spectre.Console.Rule());
            AnsiConsole.WriteLine($"{game.CurrentPosition.PlayerToMove} to move.");
            AnsiConsole.WriteLine($"Move: {move.Move.ToString()}");
            AnsiConsole.WriteLine($"FEN: {FenSerializer.Default.Serialize(game.CurrentPosition)}");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new ChessBoardView(game.CurrentPosition));
        }
    }
}