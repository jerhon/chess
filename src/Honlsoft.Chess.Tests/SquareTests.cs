namespace Honlsoft.Chess.Tests; 

public class SquareTests {


    [Fact]
    public void Parse_PieceAndSquare() {
        var square = Square.Parse("Pc4");
        square.Should().BeEquivalentTo(new Square( SquareName.Parse("c4"), new Piece( PieceType.Pawn, PieceColor.White )));
    }

    [Fact]
    public void Parse_SquareOnly() {
        var square = Square.Parse("e4");
        square.Should().BeEquivalentTo(new Square(SquareName.Parse("e4"), null));
    }

    [Fact]
    public void Parse_InvalidSquare_Throws() {

        Assert.Throws<FormatException>(() => {
            var square = Square.Parse("Pb4 ");
        });

    }
}