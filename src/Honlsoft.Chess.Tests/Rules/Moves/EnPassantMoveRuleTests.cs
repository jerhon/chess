using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class EnPassantMoveRuleTests {

    [Fact]
    public void IsApplicable_True_ForPawnsAndEnPassantBoards() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.EnPassant = SquareName.Parse("b6");
        chessBoard.AddPieces("Pc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeTrue();
    }

    [Fact]
    void IsApplicable_False_WhenNotAPawn() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.EnPassant = SquareName.Parse("b6");
        chessBoard.AddPieces("Nc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsApplicable_False_NoEnPassant() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.AddPieces("Pc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("b5", "Pc4")]
    [InlineData("b5", "Pa4")]
    [InlineData("b5", "pa6")]
    [InlineData("b5", "pc6")]
    public void GetPossibleMoves_ReturnsEnpassantSquare(string enPassantSquareName, string pawnNotation) {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.AddPieces(pawnNotation);
        chessBoard.EnPassant = SquareName.Parse(enPassantSquareName);

        var candidateMoves = rule.GetPossibleMoves(chessBoard, SquareName.Parse(pawnNotation.Substring(1)));
        candidateMoves.Should().BeEquivalentTo(new[] {
            chessBoard.EnPassant
        });

    }
}