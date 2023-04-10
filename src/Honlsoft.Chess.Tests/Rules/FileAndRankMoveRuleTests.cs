using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules; 

public class FileAndRankMoveRuleTests {
    
    [Fact]
    public void IsApplicable_RookAndQueen_ReturnsTrue() {
        
        var serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize("Rc3:rc4:Qd3:qd4");

        var rule = new FileAndRankMoveRule();
        
        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c3"));
        Assert.True(result);

        result = rule.IsApplicable(chessBoard, SquareName.Parse("c4"));
        Assert.True(result);
        
        result = rule.IsApplicable(chessBoard, SquareName.Parse("d3"));
        Assert.True(result);
        
        result = rule.IsApplicable(chessBoard, SquareName.Parse("d4"));
        Assert.True(result);
    }


    [Theory]
    [InlineData("Rd4", "d4", "d1:d2:d3:d5:d6:d7:d8:a4:b4:c4:e4:f4:g4:h4")]
    public void GetCandidateMoves_Theories(string boardSetup, string position, string candidateSquares) {
        RuleTest.TestRuleCandidates(boardSetup, position, candidateSquares, new FileAndRankMoveRule());
    }
}