namespace Honlsoft.Chess.Rules; 

public class EnPassantRule : IMoveRule {

    public bool IsApplicable(IChessBoard chessBoard, SquareName from) {
        var currentSquare = chessBoard.GetSquare(from);
        return currentSquare is { Piece: { Type: PieceType.Pawn } } && chessBoard.EnPassant is not null;
    }
    
    public SquareName[] GetPossibleMoves(IChessBoard chessBoard, SquareName from) {

        if (!IsApplicable(chessBoard, from)) {
            return Array.Empty<SquareName>();
        }

        var currentSquare = chessBoard.GetSquare(from);
        var direction = currentSquare!.Piece!.Color == PieceColor.White ? 1 : -1;

        if (currentSquare.Name.Add( 1, direction) == chessBoard.EnPassant
            || currentSquare.Name.Add(-1, direction) == chessBoard.EnPassant) {
            return new[] { chessBoard.EnPassant! };
        }

        return Array.Empty<SquareName>();
    }
}