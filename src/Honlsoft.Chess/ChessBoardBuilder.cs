namespace Honlsoft.Chess; 

/// <summary>
/// Builds a chess board with pieces on it.
/// </summary>
public class ChessBoardBuilder {

    private readonly List<Square> _squares = new List<Square>();

    public ChessBoardBuilder WithSquare(Square square) {
        _squares.Add(square);
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

    public ChessBoard Build() {
        return new ChessBoard(_squares.ToArray());
    }
}