using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess;

public class GameRulesEngine {

    private readonly IEnumerable<IMoveRule> _moveRules;

    public GameRulesEngine(IEnumerable<IMoveRule> moveRules) {
        _moveRules = moveRules;
    }

    public ChessGameResult CalculateResult(IChessGame gameState) {
        var currentPlayerMoves = new MoveCalculator(gameState.CurrentBoard, _moveRules, gameState.CurrentPlayer);
        var otherPlayerMoves = new MoveCalculator(gameState.CurrentBoard, _moveRules, gameState.CurrentPlayer);
        var kingSquare = currentPlayerMoves.GetKingSquare();
        var kingMoves = GetAllMoves(gameState.CurrentBoard, kingSquare.Name);

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
    public ChessMove? GetMove(IChessBoard chessBoard, SquareName from, SquareName to) => GetAllMoves(chessBoard, from).FirstOrDefault((m) => m.To == to);

    
    /// <summary>
    /// Validates a move in a chess game.
    /// </summary>
    /// <param name="gameState">The current state of the chess game.</param>
    /// <param name="from">The chessboard square where the move begins.</param>
    /// <param name="to">The chessboard square where the move ends.</param>
    /// <returns>A tuple with two elements. The first is a boolean indicating whether the move is valid or not. If the move is invalid, the second string element contains the reason for its invalidity. If the move is valid, this element will be null.</returns>
    public (ChessMove? Move, string? Reason) IsValidMove(IChessGame gameState, SquareName from, SquareName to) {
        
        var gameResult = CalculateResult(gameState);
        if (gameResult == ChessGameResult.Checkmate || gameResult == ChessGameResult.Stalemate) {
            return (null, $"You are in {gameResult.ToString().ToLower()}. The game is over.");
        }
        
        // Needs to be the current player's piece to move.
        if (!IsCurrentPlayerPiece(gameState, from)) {
            return (null, "This piece is not your color.");
        }

        // Is player in check, can only move the king.
        if (gameResult == ChessGameResult.Check) {
            var square = gameState.CurrentBoard.GetSquare(from);
            if (square is not { Piece: {Type: PieceType.King }} || square?.Piece?.Color != gameState.CurrentPlayer) {
                return (null, "Your King is in check. You must move your king out of check.");
            }
        }


        var chessMove = GetMove(gameState.CurrentBoard, from, to);
        
        // If the move is invalid, don't allow it.
        if (chessMove is null) {
            return (chessMove, $"The piece on {from} cannot move to square {to}.");
        }

        return (chessMove, null);
    }
    
        
    private IEnumerable<ChessMove> GetAllMoves(IChessBoard chessBoard, SquareName from) => _moveRules
        .Where(movementRule => movementRule.IsApplicable(chessBoard, from))
        .SelectMany(movementRule => movementRule.GetCandidateMoves(chessBoard, from));

    private bool IsKingInCheck(MoveCalculator otherPlayerMoves, Square kingSquare) => otherPlayerMoves.GetMoveCount(kingSquare.Name) > 0;

    private bool CanKingMoveSafely(MoveCalculator otherPlayerMoves, IEnumerable<ChessMove> kingMoves) =>
        kingMoves.Any(move => otherPlayerMoves.GetMoveCount(move.To) == 0);

    
    private bool IsCurrentPlayerPiece(IChessGame gameState, SquareName from) {
        var square = gameState.CurrentBoard.GetSquare(from);
        return square.Piece?.Color == gameState.CurrentPlayer;
    }
}