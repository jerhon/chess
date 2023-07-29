using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Honlsoft.Chess.Rules;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame : IChessGame {

    private readonly List<PlayerTurn> _gameMoves;
    private readonly GameRulesEngine _rulesEngine;
    private readonly ChessBoardBuilder _chessBoard;
    
    public ChessGame(IChessBoard initialChessBoard, GameRulesEngine rulesEngine) {
        _chessBoard = new ChessBoardBuilder();
        _rulesEngine = rulesEngine;
        _gameMoves = new List<PlayerTurn>();
        Turns = new ReadOnlyCollection<PlayerTurn>(_gameMoves);
    }
    
    /// <summary>
    /// The moves in the chess game.
    /// </summary>
    public IReadOnlyList<PlayerTurn> Turns { get; }


    public PieceColor CurrentPlayer { get; } = PieceColor.White;

    public IChessBoard CurrentBoard => _chessBoard;

    /// <summary>
    /// Trys to move a piece in the game.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public (bool CanMove, string? Reason) MovePiece(SquareName from, SquareName to) {
        
        var (move, reason) = _rulesEngine.IsValidMove(this, from, to);
        if (move == null) {
            return (false, reason);
        }

        _chessBoard.Move(move);
        
        _gameMoves.Add(new PlayerTurn(_chessBoard.Build(), move, CurrentPlayer));
        
        
        // Change the color
        var nextColor = NextColor();

        
        return (true, null);
    }
    
    private PieceColor NextColor() {
        if (CurrentPlayer == PieceColor.Black) {
            return PieceColor.White;
        } else {
            return PieceColor.Black;
        }
    }
}