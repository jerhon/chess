using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Builds a chess board with pieces on it.
/// </summary>
public class ChessBoardBuilder : IChessBoard {

    private readonly Dictionary<SquareName, Square> _squares = new();
    
    public SquareName? EnPassantTarget { get; private set; }

    public ChessBoardBuilder WithSquare(Square square) {
        _squares[square.Name] = square;
        return this;
    } 

    public ChessBoardBuilder WithSquare(SquareName squareName, PieceType type, PieceColor color) {
        var square = new Square(squareName, new Piece(type, color));
        return WithSquare(square);
    }

    public ChessBoardBuilder WithSquare(string pieceAndSquareNotation) {
        var square = Square.Parse(pieceAndSquareNotation);
        return WithSquare(square);
    }

    public ChessBoardBuilder FromBoard(IChessBoard chessBoard) {
        this.EnPassantTarget = chessBoard.EnPassantTarget;
        foreach (var squareName in SquareName.AllSquares()) {
            var square = chessBoard.GetSquare(squareName);
            if (square.HasPiece) {
                WithSquare(square);
            }
        }
        return this;
    }

    /// <summary>
    /// Moves the piece on one square to another.
    /// </summary>
    /// <param name="from">from</param>
    /// <param name="to">to</param>
    /// <returns></returns>
    public ChessBoardBuilder Move(SquareName from, SquareName to) {
        // Move the piece off the square... there is nothing there anymore
        if (_squares.ContainsKey(from)) {
            var fromSquare = _squares[from];
            var toSquare = new Square(to, fromSquare.Piece);
            _squares.Remove(from);
            _squares[to] = toSquare;
        }
        
        return this;
    }

    public ChessBoardBuilder Move(ChessMove move) {
        Move(move.From, move.To);
        if (move.EnPassantCapture != null) {
            RemovePiece(move.EnPassantCapture);
        }
        WithEnPassantTarget(move.EnPassantTarget);
        return this;
    }

    public ChessBoardBuilder RemovePiece(SquareName squareName) {
        if (this._squares.ContainsKey(squareName)) {
            this._squares.Remove(squareName);
        }
        return this;
    }

    public ChessBoardBuilder WithEnPassantTarget(SquareName? squareName) {
        EnPassantTarget = squareName;
        return this;
    }
    
    public ChessBoard Build() {
        return new ChessBoard(_squares.Values.ToArray()) {  };
    }
    public Square GetSquare(SquareName squareName) {
        if (_squares.TryGetValue(squareName, out var square)) {
            return square;
        }
        return new Square(squareName, null);
    }
    
    
    /// <summary>
    /// Creates a new game with the chess pieces in their standard positions.
    /// </summary>
    /// <returns></returns>
    public ChessBoardBuilder AddStandardGamePieces() {
        foreach (var position in SquareName.AllSquares()) {
            var color = GetInitialColor(position);
            var piece = GetInitialPieceType(position);

            if (piece != null && color != null) {
                WithSquare(position, piece.Value, color.Value);
            }
        }
        return this;
    }
    
    
    private static PieceType? GetInitialPieceType(SquareName position) =>
        (position.File.Name, position.Rank.Number) switch {
            ('a' or 'h', 1 or 8) => PieceType.Rook,
            ('b' or 'g', 1 or 8) => PieceType.Knight,
            ('c' or 'f', 1 or 8) => PieceType.Bishop,
            ('d', 1 or 8) => PieceType.Queen,
            ('e', 1 or 8) => PieceType.King,
            (_, 7 or 2) => PieceType.Pawn,
            _ => null
        };

    private static PieceColor? GetInitialColor(SquareName position) =>
        position.Rank.Number switch {
            1 or 2 => PieceColor.White,
            7 or 8 => PieceColor.Black,
            _ => null
        };
}