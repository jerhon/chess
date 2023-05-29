namespace Honlsoft.Chess.Rules; 

/// <summary>
/// Moves pieces in diagonals.
/// </summary>
public class DiagonalMoveRule : IMoveRule {

    /// <summary>
    /// Only bishops and queens move diagonally.
    /// </summary>
    /// <param name="chessBoard"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public bool IsApplicable(IChessBoard chessBoard, SquareName from) {
        var square = chessBoard.GetSquare(from);
        return square is { Piece: { Type: PieceType.Bishop or PieceType.Queen } };
    }
    
    public SquareName[] GetPossibleMoves(IChessBoard chessBoard, SquareName from) {
        List<SquareName> squares = new List<SquareName>();

        var originalSquare = chessBoard.GetSquare(from);
        if (!IsApplicable(chessBoard, from)) {
            return Array.Empty<SquareName>();
        }

        var topRightSquares = EnumerateDiagonal(from, 1, 1);
        var topLeftSquares = EnumerateDiagonal(from, 1, -1);
        var bottomRightSquares = EnumerateDiagonal(from, -1, 1);
        var bottomLeftSquares = EnumerateDiagonal(from, -1, -1);
        
        foreach (var squareEnum in new [] { topRightSquares, topLeftSquares, bottomLeftSquares, bottomRightSquares }) {
            foreach (var square in squareEnum) {
                var (cont, add) = CheckSquare(chessBoard, originalSquare.Piece.Color, square);
                if (add) {
                    squares.Add(square);
                }
                if (!cont) {
                    break;
                }
            }
        }

        return squares.ToArray();
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
    
    
    private (bool Continue, bool Add) CheckSquare(IChessBoard chessBoard, PieceColor currentColor, SquareName candidateSquareName) {
        
        var candidateSquare = chessBoard.GetSquare(candidateSquareName);
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