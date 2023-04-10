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
    [InlineData("rd8", "d8", "d7:d6:d5:d4:d3:d2:d1:a8:b8:c8:e8:f8:g8:h8")]
    [InlineData("Ra1", "a1", "b1:c1:d1:e1:f1:g1:h1:a2:a3:a4:a5:a6:a7:a8")]
    [InlineData("Rh8", "h8", "a8:b8:c8:d8:e8:f8:g8:h1:h2:h3:h4:h5:h6:h7")]
    [InlineData("Rd4:Pd3:Pd5:Pc4:Pe4", "d4", "")]
    [InlineData("Rd4:pd3:pd5:pc4:pe4", "d4", "d3:d5:c4:e4")]
    public void GetCandidateMoves_Theories(string boardSetup, string position, string candidateSquares) {
        RuleTest.TestRuleCandidates(boardSetup, position, candidateSquares, new FileAndRankMoveRule());
    }
}