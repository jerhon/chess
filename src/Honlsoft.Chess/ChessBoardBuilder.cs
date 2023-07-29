using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Builds a chess board with pieces on it.
/// </summary>
public class ChessBoardBuilder : IChessBoard {

    private readonly Dictionary<SquareName, Square> _squares = new();

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
        Move(move.FromSquare, move.ToSquare);
        if (move.EnPassantCapture != null) {
            RemovePiece(move.EnPassantCapture);
        }
        return this;
    }

    public ChessBoardBuilder RemovePiece(SquareName squareName) {
        if (this._squares.ContainsKey(squareName)) {
            this._squares.Remove(squareName);
        }
        return this;
    }
    
    public ChessBoard Build() {
        return new ChessBoard(_squares.Values.ToArray());
    }
    public Square GetSquare(SquareName squareName) {
        if (_squares.TryGetValue(squareName, out var square)) {
            return square;
        }
        return new Square(squareName, null);
    }
    public SquareName? EnPassantTarget { get; }
}