using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Honlsoft.Chess.Rules;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame {

    private readonly List<ChessGameMove> _gameMoves;
    private readonly GameRulesEngine _rulesEngine;

    public ChessGame(ChessGameState initialCurrentState, GameRulesEngine rulesEngine) {
        CurrentState = initialCurrentState;
        _rulesEngine = rulesEngine;
        _gameMoves = new List<ChessGameMove>();
        Moves = new ReadOnlyCollection<ChessGameMove>(_gameMoves);
    }
    
    /// <summary>
    /// The moves in the chess game.
    /// </summary>
    public IReadOnlyList<ChessGameMove> Moves { get; }
    
    /// <summary>
    /// The current state of the game.
    /// </summary>
    public ChessGameState CurrentState { get; private set; }
    
    /// <summary>
    /// Trys to move a piece in the game.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public (bool CanMove, string? Reason) MovePiece(SquareName from, SquareName to) {
        
        var (isValid, reason) = _rulesEngine.IsValidMove(CurrentState, from, to);
        if (!isValid) {
            return (isValid, reason);
        }
        
        // Make the move
        var newBoard = CurrentState.Board.Move(from, to);
        
        // Change the color
        var newColor = NextColor();

        var nextGameState = new ChessGameState(newBoard, newColor);

        _gameMoves.Add(new ChessGameMove(CurrentState, nextGameState, from, to));
        
        CurrentState = nextGameState;
        
        return (true, null);
    }
    
    private PieceColor NextColor() {
        if (CurrentState.CurrentColor == PieceColor.Black) {
            return PieceColor.White;
        } else {
            return PieceColor.Black;
        }
    }
}