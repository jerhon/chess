using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Honlsoft.Chess.Rules;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame : IChessGame {
    private readonly GameRules _rules;

    private readonly List<PlayerTurn> _gameMoves = new();
    private readonly ChessPositionBuilder _chessPosition;
    
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

    public IChessPosition CurrentPosition => _chessPosition;
    
    public SquareName? EnPassantTarget { get; private set; }

    public ChessGameState GameState { get; private set; }


    public ChessGame(IChessPosition initialChessPosition, PieceColor playerToMove,  GameRules rules) {
        _rules = rules;

        _chessPosition = new ChessPositionBuilder()
            .FromBoard(initialChessPosition);
        
        GameState = rules.CalculateState(initialChessPosition, playerToMove);
        
        CurrentPlayer = playerToMove;
    }

    public ChessGame(GameRules gameRules) : this(ChessPositionBuilder.StandardGame, PieceColor.White, gameRules) { }
    
    /// <summary>
    /// Trys to move a piece in the game.
    /// </summary>
    /// <param name="from">The position to move from</param>
    /// <param name="to">The position to move to</param>
    /// <returns></returns>
    public MoveResult MovePiece(SquareName from, SquareName to, PieceType? promitionPiece) {
        
        var (validationResult, move) = _rules.IsValidMove(this, from, to, promitionPiece);
        if (move == null || validationResult != MoveResult.ValidMove) {
            return validationResult;
        }
        
        _chessPosition.Move(move);
        
        _gameMoves.Add(new PlayerTurn(_chessPosition.Build(), move, CurrentPlayer));
        
        // Change the color
        CurrentPlayer = NextColor();

        // Calculate the current state of the game
        GameState = _rules.CalculateState(_chessPosition, CurrentPlayer);
        
        return validationResult;
    }
    
    /// <summary>
    /// Determines the next color to play.
    /// </summary>
    /// <returns>The next color to play.</returns>
    private PieceColor NextColor() {
        if (CurrentPlayer == PieceColor.Black) {
            return PieceColor.White;
        } else {
            return PieceColor.Black;
        }
    }

    public SquareName[] GetCandidateMoves(SquareName squareName)  => _rules.GetMoves(_chessPosition, squareName).Select((m) => m.To).ToArray();
}