namespace Honlsoft.Chess.Tests; 

public class ChessBoardBuilderTests {


    [Fact]
    public void WithSquare_AddsSquare_BuildsChessBoard() {

        var chessBoardBuilder = new ChessPositionBuilder();

        chessBoardBuilder
            .WithSquare("Pb4")
            .WithSquare("pc4");

        var chessBoard = chessBoardBuilder.Build();

        var squarePb4 = chessBoard.GetSquare(SquareName.Parse("b4"));
        var squarepc4 = chessBoard.GetSquare(SquareName.Parse("c4"));
        var squareg5 = chessBoard.GetSquare(SquareName.Parse("g5"));
        
        squarePb4.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("Pb4"));
        squarepc4.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("pc4"));
        squareg5.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("g5"));

    }
}