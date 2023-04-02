namespace Honlsoft.Chess.Tests; 

public class PieceTests {

    [Theory]
    [InlineData(PieceType.Bishop, PieceColor.White, "B")]
    [InlineData(PieceType.Knight, PieceColor.White, "N")]
    [InlineData(PieceType.Rook, PieceColor.White, "R")]
    [InlineData(PieceType.King, PieceColor.White, "K")]
    [InlineData(PieceType.Queen, PieceColor.White, "Q")]
    [InlineData(PieceType.Pawn, PieceColor.White, "P")]
    [InlineData(PieceType.Bishop, PieceColor.Black, "b")]
    [InlineData(PieceType.Knight, PieceColor.Black, "n")]
    [InlineData(PieceType.Rook, PieceColor.Black, "r")]
    [InlineData(PieceType.King, PieceColor.Black, "k")]
    [InlineData(PieceType.Queen, PieceColor.Black, "q")]
    [InlineData(PieceType.Pawn, PieceColor.Black, "p")]
    public void ToString_ReturnsCorrectValue(PieceType pieceType, PieceColor pieceColor, string expected) {
        Piece piece = new Piece(pieceType, pieceColor);
        var result = piece.ToString();
        Assert.Equal(expected, result);
    }
}