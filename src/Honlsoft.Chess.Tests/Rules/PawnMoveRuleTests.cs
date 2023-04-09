using System.Data;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules; 

public class PawnMoveRuleTests {

    [Theory]
    [InlineData("Pb2", "b2", "b3:b4")]
    [InlineData("Pb2:pa3:pc3", "b2", "b3:b4:a3:c3")]
    [InlineData("Pb8", "b8", "")]
    public void GetPossibleMoves_Success(string boardSetup, string positionToEvaluate, string expectedMoves) {
        
        SimpleSerializer serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize(boardSetup);
        
        PawnMoveRule rule = new PawnMoveRule();
        var actualMoves = rule.GetPossibleMoves(chessBoard, SquareName.Parse(positionToEvaluate));

        SquareName[] expectedMovesArray = Array.Empty<SquareName>();
        if (!string.IsNullOrEmpty(expectedMoves)) {
            expectedMovesArray = expectedMoves.Split(":").Select((m) => SquareName.Parse(m)).ToArray();
        }
        
        Assert.Equal(expectedMovesArray.Length, actualMoves.Length);
        Assert.Equal(expectedMovesArray, actualMoves);
    }
}