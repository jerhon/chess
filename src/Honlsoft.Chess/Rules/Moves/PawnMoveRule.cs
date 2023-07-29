namespace Honlsoft.Chess.Rules;


// TODO: Have to deal with en passant.  Probably best to add an interface which passes in whether there was an eligible move, or we can just create an en passant rule

public class PawnMoveRule : IMoveRule {
    
    /// <summary>
    /// Checks if a piece is valid on a chess board for a square.
    /// </summary>
    /// <param name="chessBoard">The chess board.</param>
    /// <param name="from">The piece where the chess board is from.</param>
    /// <returns>True if the piece is valid for the move.</returns>
    public bool IsApplicable(IChessBoard chessBoard, SquareName from) {
        var square = chessBoard.GetSquare(from);
        return square is { Piece: { Type: PieceType.Pawn } };
    }
    
    /// <summary>
    /// Returns possible moves for a pawn at the given position on the board.
    /// </summary>
    /// <param name="chessBoard"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ChessMove[] GetCandidateMoves(IChessBoard chessBoard, SquareName from) {
        if (!IsApplicable(chessBoard, from)) {
            return Array.Empty<ChessMove>();
        }

        List<SquareName> squares = new List<SquareName>();
        var square = chessBoard.GetSquare(from);
        var pieceColor = square!.Piece!.Color;
        bool startingRank = IsInStartingPosition(square);
        int direction = square.Piece.Color == PieceColor.Black ? -1 : 1;
        int maxSpaces = startingRank ? 2 : 1;
        for (int i = 1; i < maxSpaces + 1; i++) {
            int move = i * direction;
            var newSquareName = from.Add(0, move);
            if (newSquareName == null) {
                break;
            }
            var newSquare = chessBoard.GetSquare(newSquareName);
            if (newSquare.HasPiece) {
                break;
            }
            squares.Add(newSquareName);
        }
        
        var leftCapture = from.Add(-1, direction);
        if (leftCapture != null) {
            var leftSquare = chessBoard.GetSquare(leftCapture);
            if (leftSquare?.Piece?.IsOpponent(pieceColor) ?? false) {
                squares.Add(leftCapture);
            }
        }

        var rightCapture = from.Add(1, direction);
        if (rightCapture != null) {
            var rightSquare = chessBoard.GetSquare(rightCapture);
            if (rightSquare?.Piece?.IsOpponent(pieceColor) ?? false) {
                squares.Add(rightCapture);
            }
        }
        
        return squares.Select((to) => new ChessMove(from, to)
        {
            EnPassantTarget = GetEnPassantTarget(from, to)
        }).ToArray();
    }

    private SquareName? GetEnPassantTarget(SquareName from, SquareName to) {
        if (from.Rank.Distance(to.Rank) > 1) {
            return new SquareName(from.File, from.Rank.Add(1));
        }
        if (from.Rank.Distance(to.Rank) > 1) {
            return new SquareName(from.File, from.Rank.Add(-1));
        }
        return null;
    }

    private bool IsInStartingPosition(Square square)
    {
        return square is { Name: { Rank: { Number: 2 } }, Piece: { Color: PieceColor.White } }
            or { Name: {Rank: { Number: 7 } }, Piece: { Color: PieceColor.Black } };
    }
}