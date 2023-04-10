using System.Data;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules; 

public class PawnMoveRuleTests {

    [Theory]
    [InlineData("Pb2", "b2", "b3:b4")]
    [InlineData("Pb2:pa3:pc3", "b2", "b3:b4:a3:c3")]
    [InlineData("Pb8", "b8", "")]
    public void GetPossibleMoves_MatchesExpectedMoves(string boardSetup, string positionToEvaluate, string expectedMoves) {
        
        RuleTest.TestRuleCandidates(boardSetup, positionToEvaluate, expectedMoves, new PawnMoveRule());
        
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


        foreach (var file in File.AllFiles) {
            actual = rule.IsApplicable(chessBoard, new SquareName(file, Rank.Rank1));
            Assert.False(actual);

            actual = rule.IsApplicable(chessBoard, new SquareName(file, Rank.Rank2));
            Assert.False(actual);
        }
        
    }
}