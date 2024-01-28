namespace Honlsoft.Chess.Tests; 

public class FakeChessPosition : IChessPosition {

    private readonly IDictionary<string, Square> _squares = new Dictionary<string, Square>();
    
    public FakeChessPosition AddPieces(params string[] squareNotations) 
    {
        foreach (var squareNotation in squareNotations) {
            var square = Square.Parse(squareNotation);
            _squares[square.Name.ToString()] = square;
        }
        return this;
    }
    
    public Square GetSquare(SquareName squareName) {
        if (_squares.TryGetValue(squareName.ToString(), out var square)) {
            return square;
        } else {
            return new Square(squareName, null);
        }
    }
    
    public SquareName? EnPassantTarget { get; set; }
    public PieceColor PlayerToMove { get; set; }
    public int FullMoves { get; set;  }
    public int HalfMoves { get; set; }

    public CastlingSide[] GetCastlingRights(PieceColor playerColor)
    {
        throw new NotImplementedException();
    }

    public bool CanCastle(PieceColor playerColor, CastlingSide castleSide) {
        throw new NotImplementedException();
    }
}