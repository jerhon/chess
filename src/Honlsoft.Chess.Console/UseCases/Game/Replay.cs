using Honlsoft.Chess.Serialization;
using Spectre.Console;

namespace Honlsoft.Chess.Console.UseCases;

public class Replay {
    
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
            
            AnsiConsole.WriteLine($"Move: {move.ToString()}");
            AnsiConsole.WriteLine($"FEN: {FenSerializer.Default.Serialize(game.CurrentPosition)}");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new ChessBoardView(game.CurrentPosition));
        }
    }
}