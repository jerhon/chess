namespace Honlsoft.Chess.Rules; 

public class EnPassantRule : IMoveRule {

    public bool IsApplicable(IChessBoard chessBoard, SquareName from) {
        var currentSquare = chessBoard.GetSquare(from);
        return currentSquare is { Piece: { Type: PieceType.Pawn } } && chessBoard.EnPassantTarget is not null;
    }
    
    public ChessMove[] GetCandidateMoves(IChessBoard chessBoard, SquareName from) {

        if (!IsApplicable(chessBoard, from)) {
            return Array.Empty<ChessMove>();
        }

        var currentSquare = chessBoard.GetSquare(from);
        var direction = currentSquare!.Piece!.Color == PieceColor.White ? 1 : -1;

        if (currentSquare.Name.Add( 1, direction) == chessBoard.EnPassantTarget
            || currentSquare.Name.Add(-1, direction) == chessBoard.EnPassantTarget) {

            var enPassantCapture = new SquareName(chessBoard.EnPassantTarget.File, from.Rank);
            
            return new[] {
                new ChessMove(from, chessBoard!.EnPassantTarget)
                {
                    EnPassantCapture = enPassantCapture
                }
            };
        }

        return Array.Empty<ChessMove>();
    }
}