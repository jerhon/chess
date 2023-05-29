namespace Honlsoft.Chess.Tests; 

public class FakeChessBoard : IChessBoard {

    private readonly IDictionary<string, Square> _squares = new Dictionary<string, Square>();
    
    public FakeChessBoard AddPieces(params string[] squareNotations) 
    {
        foreach (var squareNotation in squareNotations) {
            var square = Square.Parse(squareNotation);
            _squares[square.Name.ToString()] = square;
        }
        return this;
    }
    
    public Square GetSquare(SquareName squareName) {
        if (_squares.TryGetValue(squareName.ToString(), out var square)) {
            return _squares[squareName.ToString()];
        } else {
            return new Square(squareName, null);
        }
    }
}