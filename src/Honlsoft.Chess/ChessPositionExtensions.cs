namespace Honlsoft.Chess;

public static class ChessPositionExtensions {
    
    
    public static SquareName GetKingSquare(this IChessPosition position, PieceColor color) {
        
        foreach (var square in SquareName.AllSquares()) {
            var piece = position.GetSquare(square).Piece;
            if (piece != null && piece.Color == color && piece.Type == PieceType.King) {
                return square;
            }
        }

        throw new InvalidOperationException("No king found.");
    }
}