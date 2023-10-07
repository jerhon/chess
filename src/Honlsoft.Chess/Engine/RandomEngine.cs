using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Engine; 

/// <summary>
/// Picks a random move for one of it's random pieces.
/// </summary>
public class RandomEngine(ChessGame chessGame) : IChessEngine {
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chessBoard"></param>
    /// <param name="moves"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartGameAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
    
    public Task<ChessMove> SuggestMoveAsync(CancellationToken cancellationToken) {

        var chessBoard = chessGame.CurrentBoard;
        var currentPlayer = chessGame.CurrentPlayer;
        
        
        // TODO: need to take additional steps into consideration (player is in check, etc)
        
        // Find all moves for the curent player
        var allCandidateMoves = SquareName.AllSquares()
            .Select(square => chessBoard.GetSquare(square))
            .Where(s => s?.Piece?.Color == currentPlayer)
            .SelectMany(s => chessGame.GetCandidateMoves(s.Name).Select((m) => new { From = s, To = m}))
            .ToArray();

        // Get a random square
        var move = Random.Shared.GetItems(allCandidateMoves, 1)[0];

        return Task.FromResult(new ChessMove(move.From.Name, move.To));
    }
}