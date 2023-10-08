namespace Honlsoft.Chess; 

/// <summary>
/// Potential different states of a chess game.
/// </summary>
public enum ChessGameState {
    Check,
    Checkmate,
    Stalemate,
    PlayerToMove,
}