using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Engine; 

/// <summary>
/// Picks a random move for one of it's random pieces.
/// </summary>
public class RandomEngine(ChessGame chessGame) : IChessEngine {
    

    /// <summary>
    /// Initialize the chess engine.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartGameAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
    public void AddMove(string moveString) {
        
    }
    public Task UpdatePositionAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
    public Task UpdatePositionAsync(string moveString, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task<EngineSuggestion> SuggestMoveAsync(CancellationToken cancellationToken) {

        var chessBoard = chessGame.CurrentPosition;
        var currentPlayer = chessGame.CurrentPosition.PlayerToMove;
        
        
        // TODO: need to take additional steps into consideration (player is in check, etc)
        
        
        // Find all moves for the current player
        var allCandidateMoves = SquareName.AllSquares()
            .Select(square => chessBoard.GetSquare(square))
            .Where(s => s?.Piece?.Color == currentPlayer)
            .SelectMany(s => chessGame.GetCandidateMoves(s.Name).Select((m) => new { From = s, To = m}))
            .ToArray();

        // Get a random square
        var move = Random.Shared.GetItems(allCandidateMoves, 1)[0];

        return Task.FromResult(new EngineSuggestion(move.From.Name, move.To));
    }
}