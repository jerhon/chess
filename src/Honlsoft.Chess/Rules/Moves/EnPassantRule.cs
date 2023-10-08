namespace Honlsoft.Chess.Rules; 

public class EnPassantRule : IMoveRule {

    public bool IsApplicable(IChessPosition chessPosition, SquareName from) {
        var currentSquare = chessPosition.GetSquare(from);
        return currentSquare is { Piece: { Type: PieceType.Pawn } } && chessPosition.EnPassantTarget is not null;
    }
    
    public IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from) {

        if (!IsApplicable(chessPosition, from)) {
            return [];
        }

        var currentSquare = chessPosition.GetSquare(from);
        var direction = currentSquare!.Piece!.Color == PieceColor.White ? 1 : -1;

        if (chessPosition.EnPassantTarget != null && (
            currentSquare.Name.Add( 1, direction) == chessPosition.EnPassantTarget
            || currentSquare.Name.Add(-1, direction) == chessPosition.EnPassantTarget)) {

            return [
                new SimpleMove(from, chessPosition!.EnPassantTarget)
            ];
        }

        return [];
    }
}