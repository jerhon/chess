namespace Honlsoft.Chess.Rules; 

public class KnightMoveRule : IMoveRule {

    public bool IsApplicable(IChessPosition chessPosition, SquareName from) {
        var square = chessPosition.GetSquare(from);
        return square is { Piece: { Type: PieceType.Knight } };
    }
    public IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from) {

        if (chessPosition == null) {
            throw new ArgumentNullException(nameof(chessPosition));
        }
        if (from == null) {
            throw new ArgumentNullException(nameof(from));
        }

        if (!IsApplicable(chessPosition, from)) {
            return [];
        }

        var square = chessPosition.GetSquare(from);

        var possibleSquares = new[] {
            from.Add(2, 1),
            from.Add(2, -1),
            from.Add(-2, 1),
            from.Add(-2, -1),
            from.Add(1, 2),
            from.Add(-1, 2),
            from.Add(1, -2),
            from.Add(-1, -2)
        };
        
        // Need to check for threats...

        var squares = possibleSquares.Where((squareName) => CanMove(chessPosition, squareName, square!.Piece!.Color));
        return squares.Where((s) => s != null).Select((s) => new SimpleMove(from, s)).ToArray()!;
    }

    private bool CanMove(IChessPosition chessPosition, SquareName? squareName, PieceColor color) {
        if (squareName == null) {
            return false;
        }
        var square = chessPosition.GetSquare(squareName);
        return square.Piece == null || (square?.Piece?.IsOpponent(color) ?? false);
    }
    
}