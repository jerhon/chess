using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGameEngine {

    private readonly IEnumerable<IMoveRule> _moveRules;

    public ChessGameEngine(ChessGameState initialGameState, IEnumerable<IMoveRule> moveRules) {
        GameState = initialGameState;
        _moveRules = moveRules;
    }

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public ChessGameState GameState { get; private set; }
    
    /// <summary>
    /// Trys to move a piece in the game.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public (bool CanMove, string? Reason) MovePiece(SquareName from, SquareName to) {

        // Needs to be the current player's piece to move.
        if (!IsCurrentPlayerPiece(from)) {
            return (false, "This piece is not your color.");
        }

        // Is player in check, can only move the king.
        
        
        // If the move is invalid, don't allow it.
        if (!CanMove(from, to)) {
            return (false, $"The piece on {from} cannot move to square {to}.");
        }

        // Make the move
        var newBoard = GameState.Board.Move(from, to);
        
        // Change the color
        var newColor = NextColor();
        
        // Should evaluate for Checkmate or Draw here.
        
        return (true, null);
    }
    
    /// <summary>
    /// Determines if the current player can move to a square.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private bool CanMove(SquareName from, SquareName to) {
        foreach (var movementRule in _moveRules) {
            if (movementRule.IsApplicable(GameState.Board, from)) {
                var movements = movementRule.GetPossibleMoves(GameState.Board, from);
                if (movements.Any((m) => m == to)) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsCurrentPlayerPiece(SquareName from) {
        var square = GameState.Board.GetSquare(from);
        return square.Piece?.Color == GameState.CurrentColor;
    }
    
    private PieceColor NextColor() {
        if (GameState.CurrentColor == PieceColor.Black) {
            return PieceColor.White;
        } else {
            return PieceColor.Black;
        }
    }
}