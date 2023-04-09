using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization; 

public class SimpleSerializerTests {



    [Fact]
    public void Serialize_ChessBoard() 
    {
        SimpleSerializer serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize("Pa2:Rb4:qc8:bd3");

        var square = chessBoard.GetSquare(SquareName.Parse("a2"));
        Assert.NotNull(square.Piece);
        Assert.Equal(PieceType.Pawn, square.Piece.Type);
        Assert.Equal(PieceColor.White, square.Piece.Color);

        square = chessBoard.GetSquare(SquareName.Parse("b4"));
        Assert.NotNull(square.Piece);
        Assert.Equal(PieceType.Rook, square.Piece.Type);
        Assert.Equal(PieceColor.White, square.Piece.Color);
        
        square = chessBoard.GetSquare(SquareName.Parse("c8"));
        Assert.NotNull(square.Piece);
        Assert.Equal(PieceType.Queen, square.Piece.Type);
        Assert.Equal(PieceColor.Black, square.Piece.Color);

        square = chessBoard.GetSquare(SquareName.Parse("d3"));
        Assert.NotNull(square.Piece);
        Assert.Equal(PieceType.Bishop, square.Piece.Type);
        Assert.Equal(PieceColor.Black, square.Piece.Color);

        // we only have 4 squares with pieces in it.
        Assert.Equal(4, chessBoard.Count((s) => s.Piece != null));
    }
}