using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;
using Honlsoft.Chess.Uci.Engine;
using Spectre.Console;

namespace Honlsoft.Chess.Console.UseCases;

public class PlayGame(IServiceProvider serviceProvider)
{
    public async Task PlayAsync()
    {
        var gameFactory = new ChessGameFactory();
        var game = gameFactory.CreateStandardGame();
        
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
    }
}