namespace Honlsoft.Chess.Rules; 

public class KnightMoveRule : IMoveRule {

    public bool IsApplicable(IChessBoard chessBoard, SquareName from) {
        var square = chessBoard.GetSquare(from);
        return square is { Piece: { Type: PieceType.Knight } };
    }
    public CandidateMove[] GetPossibleMoves(IChessBoard chessBoard, SquareName from) {

        if (chessBoard == null) {
            throw new ArgumentNullException(nameof(chessBoard));
        }
        if (from == null) {
            throw new ArgumentNullException(nameof(from));
        }

        if (!IsApplicable(chessBoard, from)) {
            return Array.Empty<CandidateMove>();
        }

        var square = chessBoard.GetSquare(from);

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

        var squares = possibleSquares.Where((squareName) => CanMove(chessBoard, squareName, square!.Piece!.Color));
        return squares.Where((s) => s != null).Select((s) => new CandidateMove(s)).ToArray()!;
    }

    private bool CanMove(IChessBoard chessBoard, SquareName? squareName, PieceColor color) {
        if (squareName == null) {
            return false;
        }
        var square = chessBoard.GetSquare(squareName);
        return square.Piece == null || (square?.Piece?.IsOpponent(color) ?? false);
    }
    
}