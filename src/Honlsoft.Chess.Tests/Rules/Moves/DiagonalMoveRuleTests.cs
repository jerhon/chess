using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class DiagonalMoveRuleTests {

    [Fact]
    public void IsApplicable_Bishop_ReturnsTrue() {
        FakeChessBoard chessBoard = new FakeChessBoard().AddPieces("Bd4");
        DiagonalMoveRule moveRule = new DiagonalMoveRule();
        bool result = moveRule.IsApplicable(chessBoard, SquareName.Parse("d4"));
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("Bd4", "d4", "e5:f6:g7:h8:c3:b2:a1:c5:b6:a7:e3:f2:g1")]
    public void GetPossibleMoves(string setup, string position, string expectedMoves) {
        FakeChessBoard fakeBoard = new FakeChessBoard().AddPieces(setup.Split(":"));
        DiagonalMoveRule moveRule = new DiagonalMoveRule();
        
        var moves = moveRule.GetCandidateMoves(fakeBoard, SquareName.Parse(position));
        var expected = ChessBoardUtils.CreateCandidateMoves("d4", expectedMoves.Split(":"));
        moves.Should().HaveCount(expected.Length).And.BeEquivalentTo(expected);
    }
}