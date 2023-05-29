using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Records the current state of the chess game
/// </summary>
/// <param name="Board">The current board state.</param>
/// <param name="CurrentColor">The current color to move.</param>
public record ChessGameState(ChessBoard Board, PieceColor CurrentColor) {
    
}