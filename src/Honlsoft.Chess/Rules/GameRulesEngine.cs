using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess;

public class GameRulesEngine {

    private readonly IEnumerable<IMoveRule> _moveRules;

    public GameRulesEngine(IEnumerable<IMoveRule> moveRules) {
        _moveRules = moveRules;
    }

    public ChessGameResult CalculateResult(ChessGameState gameState) {
        var currentPlayerMoves = new MoveCalculator(gameState.Board, _moveRules, gameState.CurrentColor);
        var otherPlayerMoves = new MoveCalculator(gameState.Board, _moveRules, gameState.CurrentColor);
        var kingSquare = currentPlayerMoves.GetKingSquare();
        var kingMoves = GetAllMoves(gameState.Board, kingSquare.Name);

        if (IsKingInCheck(otherPlayerMoves, kingSquare)) {
            return CanKingMoveSafely(otherPlayerMoves, kingMoves)
                ? ChessGameResult.Check
                : ChessGameResult.Checkmate;
        }

        return currentPlayerMoves.GetTotalMoves() == 0 ? ChessGameResult.Stalemate : ChessGameResult.Normal;
    }

    /// <summary>
    /// Determines if the a piece can move from one square to another.
    /// </summary>
    /// <param name="chessBoard">The chess board.</param>
    /// <param name="from">The square to move from.</param>
    /// <param name="to">The square to move to.</param>
    /// <returns></returns>
    public bool CanMove(IChessBoard chessBoard, SquareName from, SquareName to) => GetAllMoves(chessBoard, from).Any((m) => m == to);


    public bool CanEnPassant(IChessBoard chessBoard, SquareName from, SquareName to) {
        return false;
    }
    
    /// <summary>
    /// Validates a move in a chess game.
    /// </summary>
    /// <param name="gameState">The current state of the chess game.</param>
    /// <param name="from">The chessboard square where the move begins.</param>
    /// <param name="to">The chessboard square where the move ends.</param>
    /// <returns>A tuple with two elements. The first is a boolean indicating whether the move is valid or not. If the move is invalid, the second string element contains the reason for its invalidity. If the move is valid, this element will be null.</returns>
    public (bool IsValid, string? Reason) IsValidMove(ChessGameState gameState, SquareName from, SquareName to) {
        
        var gameResult = CalculateResult(gameState);
        if (gameResult == ChessGameResult.Checkmate || gameResult == ChessGameResult.Stalemate) {
            return (false, $"You are in {gameResult.ToString().ToLower()}. The game is over.");
        }
        
        // Needs to be the current player's piece to move.
        if (!IsCurrentPlayerPiece(gameState, from)) {
            return (false, "This piece is not your color.");
        }

        // Is player in check, can only move the king.
        if (gameResult == ChessGameResult.Check) {
            var square = gameState.Board.GetSquare(from);
            if (square is not { Piece: {Type: PieceType.King }} || square?.Piece?.Color != gameState.CurrentColor) {
                return (false, "Your King is in check. You must move your king out of check.");
            }
        }
        
        // If the move is invalid, don't allow it.
        if (!CanMove(gameState.Board, from, to)) {
            return (false, $"The piece on {from} cannot move to square {to}.");
        }

        return (true, null);
    }
    
        
    private IEnumerable<SquareName> GetAllMoves(IChessBoard chessBoard, SquareName from) => _moveRules
        .Where(movementRule => movementRule.IsApplicable(chessBoard, from))
        .SelectMany(movementRule => movementRule.GetPossibleMoves(chessBoard, from));

    private bool IsKingInCheck(MoveCalculator otherPlayerMoves, Square kingSquare) => otherPlayerMoves.GetMoveCount(kingSquare.Name) > 0;

    private bool CanKingMoveSafely(MoveCalculator otherPlayerMoves, IEnumerable<SquareName> kingMoves) =>
        kingMoves.Any(move => otherPlayerMoves.GetMoveCount(move) == 0);

    
    private bool IsCurrentPlayerPiece(ChessGameState gameState, SquareName from) {
        var square = gameState.Board.GetSquare(from);
        return square.Piece?.Color == gameState.CurrentColor;
    }
}