namespace Honlsoft.Chess.Rules; 

/// <summary>
/// Moves pieces in diagonals.
/// </summary>
public class DiagonalMoveRule : IMoveRule {

    /// <summary>
    /// Only bishops and queens move diagonally.
    /// </summary>
    /// <param name="chessPosition"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public bool IsApplicable(IChessPosition chessPosition, SquareName from) {
        var square = chessPosition.GetSquare(from);
        return square is { Piece: { Type: PieceType.Bishop or PieceType.Queen } };
    }
    
    public IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from) {
        List<SquareName> squares = new List<SquareName>();

        var originalSquare = chessPosition.GetSquare(from);
        if (!IsApplicable(chessPosition, from)) {
            return [];
        }

        var topRightSquares = EnumerateDiagonal(from, 1, 1);
        var topLeftSquares = EnumerateDiagonal(from, 1, -1);
        var bottomRightSquares = EnumerateDiagonal(from, -1, 1);
        var bottomLeftSquares = EnumerateDiagonal(from, -1, -1);
        
        foreach (var squareEnum in new [] { topRightSquares, topLeftSquares, bottomLeftSquares, bottomRightSquares }) {
            foreach (var square in squareEnum) {
                var (cont, add) = CheckSquare(chessPosition, originalSquare.Piece.Color, square);
                if (add) {
                    squares.Add(square);
                }
                if (!cont) {
                    break;
                }
            }
        }

        return squares.Select((s) => new SimpleMove(from, s)).ToArray();
    }


    private IEnumerable<SquareName> EnumerateDiagonal(SquareName startSquare, int fileStep, int rankStep) {
        int currentFile = 0;
        int currentRank = 0;
        
        for (int j = 0; j < 8; j++) {
            currentRank += rankStep; 
            currentFile += fileStep;

            var nextSquare = startSquare.Add(currentFile, currentRank);
            if (nextSquare != null) {
                yield return nextSquare;
            } else {
                yield break;
            }
        }
    }
    
    
    private (bool Continue, bool Add) CheckSquare(IChessPosition chessPosition, PieceColor currentColor, SquareName candidateSquareName) {
        
        var candidateSquare = chessPosition.GetSquare(candidateSquareName);
        if (candidateSquare?.Piece == null) {
            return (true, true);
        } else {
            if (candidateSquare.Piece.IsOpponent(currentColor)) {
                return (false, true);
            }

            return (false, false);
        }
    }
}