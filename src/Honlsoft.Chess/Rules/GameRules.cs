using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess;

public class GameRules(MoveRules moveRules) {

    public ChessGameState CalculateState(IChessPosition chessPosition) {
        var otherPlayerColor = Piece.GetOppositeColor(chessPosition.PlayerToMove);
        var currentPlayerMoves = moveRules.GetMoveCounter(chessPosition, chessPosition.PlayerToMove);
        var otherPlayerMoves = moveRules.GetMoveCounter(chessPosition, otherPlayerColor);
        var kingSquare = currentPlayerMoves.GetKingSquare();
        var kingMoves = moveRules.GetMoves(chessPosition, kingSquare.Name);

        if (IsKingInCheck(otherPlayerMoves, kingSquare)) {
            return CanKingMoveSafely(otherPlayerMoves, kingMoves)
                ? ChessGameState.Check
                : ChessGameState.Checkmate;
        }

        if (currentPlayerMoves.GetTotalMoves() == 0) {
            return ChessGameState.Stalemate;
        }
        
        return ChessGameState.PlayerToMove;
    }

    /// <summary>
    /// Determines if the a piece can move from one square to another.
    /// </summary>
    /// <param name="chessPosition">The chess board.</param>
    /// <param name="from">The square to move from.</param>
    /// <param name="to">The square to move to.</param>
    /// <returns></returns>
    public IChessMove? GetMove(IChessPosition chessPosition, SquareName from, SquareName to) {
        return moveRules.GetMove(chessPosition, from, to);
    }


    public IEnumerable<IChessMove> GetMoves(IChessPosition chessPosition, SquareName from) {
        return moveRules.GetMoves(chessPosition, from);
    }
    
    /// <summary>
    /// Validates a move in a chess game.
    /// </summary>
    /// <param name="gameState">The current state of the chess game.</param>
    /// <param name="from">The chessboard square where the move begins.</param>
    /// <param name="to">The chessboard square where the move ends.</param>
    /// <param name="promotionPiece">The piece to promote to, if it is eligible for promotion.</param>
    /// <returns>A tuple with two elements. The first is a boolean indicating whether the move is valid or not. If the move is invalid, the second string element contains the reason for its invalidity. If the move is valid, this element will be null.</returns>
    public (MoveResult, IChessMove?) IsValidMove(IChessGame gameState, SquareName from, SquareName to, PieceType? promotionPiece) {

        
        var gameResult = CalculateState(gameState.CurrentPosition);
        if (gameResult == ChessGameState.Checkmate || gameResult == ChessGameState.Stalemate) {
            return (MoveResult.GameOver, null);
        }
        
        // Needs to be the current player's piece to move.
        if (!IsCurrentPlayerPiece(gameState, from)) {
            return (MoveResult.PieceWrongColor, null);
        }

        // Is player in check, must move the king, or block the threat.
        if (gameResult == ChessGameState.Check) {
            var square = gameState.CurrentPosition.GetSquare(from);
            //if (square is not { Piece: {Type: PieceType.King }} || square?.Piece?.Color != gameState.CurrentPosition.PlayerToMove) {
            //    return (MoveResult.InCheckMustMoveKing, null);
            //}

            var chessPositionBuilder = new ChessPositionBuilder()
                .FromPosition(gameState.CurrentPosition)
                .Move(from, to);

            // Needs to move out of check...
            var newResult = CalculateState(chessPositionBuilder);
            if (newResult == ChessGameState.Check) {
                return (MoveResult.MustMoveOutOfCheck, null);
            }
        }
        
        var chessMove = GetMove(gameState.CurrentPosition, from, to);
        
        // If the move is invalid, don't allow it.
        if (chessMove is null) {
            return (MoveResult.NotALegalMove, null);
        }

        if (IsPromotingPawn(gameState.CurrentPosition, from, to)) {
            if (promotionPiece == null) {
                return (MoveResult.RequiresPromotion, chessMove);
            }
        }

        
        return (MoveResult.ValidMove, chessMove);
    }
    
    private bool IsKingInCheck(MoveCounter otherPlayerMoves, Square kingSquare) => otherPlayerMoves.GetMoveCount(kingSquare.Name) > 0;

    private bool IsPromotingPawn(IChessPosition position, SquareName from, SquareName to) {
        var square = position.GetSquare(from);
        if (square is { Piece: { Type: Chess.PieceType.Pawn }}) {
            return to.SquareRank == SquareRank.Rank1 || to.SquareRank == SquareRank.Rank8;
        }
        return false;
    }

    
    private bool CanKingMoveSafely(MoveCounter otherPlayerMoves, IEnumerable<IChessMove> kingMoves) =>
        kingMoves.Any(move => otherPlayerMoves.GetMoveCount(move.To) == 0);

    
    private bool IsCurrentPlayerPiece(IChessGame gameState, SquareName from) {
        var square = gameState.CurrentPosition.GetSquare(from);
        return square.Piece?.Color == gameState.CurrentPosition.PlayerToMove;
    }
}