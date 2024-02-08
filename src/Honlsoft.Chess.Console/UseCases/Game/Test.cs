using System.Diagnostics;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Serialization.Pgn;

namespace Honlsoft.Chess.Console.UseCases;

/// <summary>
/// Imports PGNs from a directory and runs through each PGN to test the game engine.
/// </summary>
public class Test {
    
    public async Task TestAsync(DirectoryInfo pgnDirectory)
    {
        var stopwatch = new Stopwatch();
        List<string> _passedFiles = new();
        List<string> _failedFiles = new();
        
        var pgnFiles = pgnDirectory.GetFiles("*.pgn");
        stopwatch.Start();
        foreach (var pgnFile in pgnFiles) {
            var pgn = await File.ReadAllTextAsync(pgnFile.FullName);
            var pgnGame = PgnSerializer.Default.Deserialize(pgn);
            var game = ChessGameFactory.Default.CreateStandardGame();
            var moves = pgnGame.Moves;
            var success = true;

            try {
                foreach (var move in moves) {
                    var result = game.Move(move.Move);
                    if (result != MoveResult.ValidMove) {
                        success = false;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                success = false;
            }

            if (success) {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write("P");
                _passedFiles.Add(pgnFile.Name);
            }
            else {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("F");
                _failedFiles.Add(pgnFile.Name);
            }
        }
        stopwatch.Stop();

        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("Passed Files:");
        foreach (var passedFile in _passedFiles) {
            System.Console.WriteLine("\t" + passedFile);
        }
        
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine("Failed Files:");
        foreach (var failedFile in _failedFiles) {
            System.Console.WriteLine("\t" + failedFile);
        }
        
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine();
        System.Console.WriteLine("Summary:");
        System.Console.WriteLine($"\tPassed: {_passedFiles.Count}");
        System.Console.WriteLine($"\tFailed: {_failedFiles.Count}");
        System.Console.WriteLine($"\tPassing Rate: {((double)_passedFiles.Count / (double)(_passedFiles.Count + _failedFiles.Count)) * 100}%");
        System.Console.WriteLine($"\tTotal Time: {stopwatch.Elapsed}");

        if (_failedFiles.Any()) {
            System.Environment.ExitCode = -1;
        }
    }
}