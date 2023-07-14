using Honlsoft.Chess.Rules;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame {

    private readonly GameRulesEngine _rulesEngine;

    public ChessGame(ChessGameState initialGameState, GameRulesEngine rulesEngine) {
        GameState = initialGameState;
        _rulesEngine = rulesEngine;
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
        
        var (isValid, reason) = _rulesEngine.IsValidMove(GameState, from, to);
        if (!isValid) {
            return (isValid, reason);
        }
        
        // Make the move
        var newBoard = GameState.Board.Move(from, to);
        
        // Change the color
        var newColor = NextColor();
        
        return (true, null);
    }
    
    private PieceColor NextColor() {
        if (GameState.CurrentColor == PieceColor.Black) {
            return PieceColor.White;
        } else {
            return PieceColor.Black;
        }
    }
}