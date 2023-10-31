namespace Honlsoft.Chess.Rules; 

/// <summary>
/// A rule which allows pieces to move across a file and a rank. A rook is an excellent example of this.  The Queen also supports this type of movement.
/// </summary>
public class FileAndRankMoveRule : IMoveRule {

    public bool IsApplicable(IChessPosition chessPosition, SquareName from) {
        var square = chessPosition.GetSquare(from);

        // Both bishops and rooks can move over ranks and files.
        return square is {
            Piece: {
                Type: PieceType.Rook or PieceType.Queen
            }
        };
    }
    
    public IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from) {

        List<SquareName> squares = new List<SquareName>();

        if (!IsApplicable(chessPosition, from)) {
            return [];
        }

        var originalSquare = chessPosition.GetSquare(from);
        
        
        var leftSquares = from.SquareRank.ToStart().Select((rank) => new SquareName(from.SquareFile, rank));
        var rightSquares = from.SquareRank.ToEnd().Select((rank) => new SquareName(from.SquareFile, rank));

        var topSquares = from.SquareFile.ToStart().Select((file) => new SquareName(file, from.SquareRank));
        var bottomSquares = from.SquareFile.ToEnd().Select((file) => new SquareName(file, from.SquareRank));
        
        foreach (var squareEnum in new [] { leftSquares, rightSquares, topSquares, bottomSquares }) {
            foreach (var square in squareEnum) {
                var (cont, add) = CheckSquare(chessPosition, originalSquare!.Piece!.Color, square);
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
