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
}