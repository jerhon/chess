namespace Honlsoft.Chess.Rules; 

/// <summary>
/// A rule which allows pieces to move across a file and a rank. A rook is an excellent example of this.  The Queen also supports this type of movement.
/// </summary>
public class FileAndRankMoveRule : IMoveRule {

    public bool IsApplicable(ChessBoard chessBoard, SquareName from) {
        var square = chessBoard.GetSquare(from);

        // Both bishops and rooks can move over ranks and files.
        return square is {
            Piece: {
                Type: PieceType.Rook or PieceType.Queen
            }
        };
    }
    
    public SquareName[] GetPossibleMoves(ChessBoard chessBoard, SquareName from) {

        List<SquareName> squares = new List<SquareName>();

        var originalSquare = chessBoard.GetSquare(from);
        
        
        var leftSquares = from.Rank.ToStart().Select((rank) => new SquareName(from.File, rank));
        var rightSquares = from.Rank.ToEnd().Select((rank) => new SquareName(from.File, rank));

        var topSquares = from.File.ToStart().Select((file) => new SquareName(file, from.Rank));
        var bottomSquares = from.File.ToEnd().Select((file) => new SquareName(file, from.Rank));
        
        foreach (var squareEnum in new [] { leftSquares, rightSquares, topSquares, bottomSquares }) {
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
    
    

    private (bool Continue, bool Add) CheckSquare(ChessBoard chessBoard, PieceColor currentColor, SquareName candidateSquareName) {
        
        var candidateSquare = chessBoard.GetSquare(candidateSquareName);
        if (candidateSquare?.Piece == null) {
            return (true, true);
        } else {
            if (candidateSquare.Piece.IsOpponent(currentColor)) {
                return (true, false);
            }

            return (false, false);
        }
    }
}
