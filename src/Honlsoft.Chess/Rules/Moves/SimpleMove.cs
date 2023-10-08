namespace Honlsoft.Chess.Rules;

/// <summary>
/// Moves a piece from one square to another.
/// </summary>
/// <param name="from">The square to move from.</param>
/// <param name="to">The square to move to.</param>
public record SimpleMove(SquareName From, SquareName To) : IChessMove {

    public void ApplyMove(ChessPositionBuilder chessPosition) {

        // Update castling rights if the King or Rook moves
        DoCastlingRights(chessPosition);
        
        chessPosition.Move(From, To);

        // Update the en-passant state.
        DoEnPassant(chessPosition);
    }
    
    public SquareName[] GetTargetedSquares() {
        return [To];
    }
    
    public PieceColor? GetPlayer(IChessPosition chessPosition) {
        return chessPosition.GetSquare(From)?.Piece?.Color;
    }

    private void DoEnPassant(ChessPositionBuilder chessPosition) {
        
        var fromSquare = chessPosition.GetSquare(From);
        var toSquare = chessPosition.GetSquare(To);
        
        
        if (chessPosition.EnPassantTarget == To) {
            var captureSquare = new SquareName(To.File, From.Rank);
            chessPosition.RemovePiece(captureSquare);
        }

        
        // En-passant
        if (fromSquare is { Piece: { Type: PieceType.Pawn } }) {
            if (fromSquare.Name.Rank == Rank.Rank2 && toSquare.Name.Rank == Rank.Rank4) {
                chessPosition.WithEnPassantTarget(fromSquare.Name with { Rank = Rank.Rank3 });
            }
            else if (fromSquare.Name.Rank == Rank.Rank7 && fromSquare.Name.Rank == Rank.Rank5) {
                chessPosition.WithEnPassantTarget(fromSquare.Name with { Rank = Rank.Rank6 });
            }
        }

    }
    
    private void DoCastlingRights(ChessPositionBuilder chessPosition) {
        
        var color = chessPosition.GetSquare(From)!.Piece!.Color;
        if (IsQueensideRook(chessPosition, From)) {
            chessPosition.WithCastlingRights(color, CastlingSide.Queenside, false);
        }
        if (IsKingsideRook(chessPosition, From)) {
            chessPosition.WithCastlingRights(color, CastlingSide.Kingside, false);
        }

    }

    public bool IsQueensideRook(IChessPosition chessPosition, SquareName from) {
        var piece = chessPosition.GetSquare(from);
        if (piece.Piece?.Type != PieceType.Rook) {
            return false;
        }

        if (from.File != File.a) {
            return false;
        }

        return from.Rank == Rank.Rank1 || from.Rank == Rank.Rank8;
    }
    
    public bool IsKingsideRook(IChessPosition chessPosition, SquareName from) {
        var piece = chessPosition.GetSquare(from);
        if (piece.Piece?.Type != PieceType.Rook) {
            return false;
        }

        if (from.File != File.h) {
            return false;
        }

        return from.Rank == Rank.Rank1 || from.Rank == Rank.Rank8;
    }

    public bool IsKing(IChessPosition chessPosition, SquareName from) {
        var piece = chessPosition.GetSquare(from);
        return PieceType.King == piece.Piece?.Type;
    }
}