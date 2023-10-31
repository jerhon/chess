using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class PawnMoveRuleTests {

    [Theory]
    [InlineData("Pb8", "b8", "")]
    public void GetPossibleMoves_MatchesExpectedMoves(string boardSetup, string positionToEvaluate, string expectedMoves) {
        
        RuleTest.TestRuleCandidates(boardSetup, positionToEvaluate, expectedMoves, new PawnMoveRule());
        
    }

    [Fact]
    public void GetPossibleMoves_InitialPositionSuccess() {
        var chessBoard = new FakeChessPosition().AddPieces("Pb2");

        var pawnMoveRule = new PawnMoveRule();

        var candidateMoves = pawnMoveRule.GetCandidateMoves(chessBoard, SquareName.Parse("b2"));

        var from = SquareName.Parse("b2");
        candidateMoves.Should().BeEquivalentTo(new object[] {
            new { From = from, To = SquareName.Parse("b3") },
            new { From = from, To = SquareName.Parse("b4") },
        });
    }
    
    [Fact]
    public void GetPossibleMoves_InitialPositionWithCaptures() {
        var chessBoard = new FakeChessPosition().AddPieces("Pb2", "pa3", "pc3");

        var pawnMoveRule = new PawnMoveRule();

        var candidateMoves = pawnMoveRule.GetCandidateMoves(chessBoard, SquareName.Parse("b2"));

        var from = SquareName.Parse("b2");
        candidateMoves.Should().BeEquivalentTo(new object[] {
            new { From = from, To = SquareName.Parse("b3") },
            new { From = from, To = SquareName.Parse("b4") },
            new { From = from, To = SquareName.Parse("a3") },
            new { From = from, To = SquareName.Parse("c3") },
        });
    }


    

    [Fact]
    public void IsApplicable_MatchesPawns() {

        SimpleSerializer serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize("Pa3:Pc4:pg8:pd5:ba1:Bb2:Nc1:nc2:Qd1:qd2:Ke1:ke2:Rf1:rf2");

        PawnMoveRule rule = new PawnMoveRule();

        var actual = rule.IsApplicable(chessBoard, SquareName.Parse("a3"));
        Assert.True(actual);

        actual = rule.IsApplicable(chessBoard, SquareName.Parse("c4"));
        Assert.True(actual);

        actual = rule.IsApplicable(chessBoard, SquareName.Parse("g8"));
        Assert.True(actual);

        actual = rule.IsApplicable(chessBoard, SquareName.Parse("d5"));
        Assert.True(actual);


        foreach (var file in SquareFile.AllFiles) {
            actual = rule.IsApplicable(chessBoard, new SquareName(file, SquareRank.Rank1));
            Assert.False(actual);

            actual = rule.IsApplicable(chessBoard, new SquareName(file, SquareRank.Rank2));
            Assert.False(actual);
        }
        
    }
}