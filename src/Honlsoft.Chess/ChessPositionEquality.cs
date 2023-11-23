namespace Honlsoft.Chess; 

/// <summary>
/// Checks that two chess positions are equal.
/// </summary>
public class ChessPositionEquality : IEqualityComparer<IChessPosition> {

    public bool AreSquaresEqual(IChessPosition x, IChessPosition y) {
        foreach (var squareName in SquareName.AllSquares()) {
            var xSquare = x.GetSquare(squareName);
            var ySquare = y.GetSquare(squareName);

            if (x != y) {
                return false;
            }
        }
        return true;
    }
    
    public bool Equals(IChessPosition x, IChessPosition y) {
        if (ReferenceEquals(x, y)) {
            return true;
        }
        if (ReferenceEquals(x, null)) {
            return false;
        }
        if (ReferenceEquals(y, null)) {
            return false;
        }
        if (x.GetType() != y.GetType()) {
            return false;
        }

        var propertiesEqual = Equals(x.EnPassantTarget, y.EnPassantTarget) && x.PlayerToMove == y.PlayerToMove && x.FullMoves == y.FullMoves && x.HalfMoves == y.HalfMoves;

        return propertiesEqual && AreSquaresEqual(x, y);
    }
    public int GetHashCode(IChessPosition obj) {
        return HashCode.Combine(obj.EnPassantTarget, (int)obj.PlayerToMove, obj.FullMoves, obj.HalfMoves);
    }
}