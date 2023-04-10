using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules; 

public class RuleTest {


    public static void TestRuleCandidates(string boardSetup, string positionToEvaluate, string expectedMoves, IMoveRule rule) {
        SimpleSerializer serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize(boardSetup);
        
        var actualMoves = rule.GetPossibleMoves(chessBoard, SquareName.Parse(positionToEvaluate));

        SquareName[] expectedMovesArray = Array.Empty<SquareName>();
        if (!string.IsNullOrEmpty(expectedMoves)) {
            expectedMovesArray = expectedMoves.Split(":").Select((m) => SquareName.Parse(m)).ToArray();
        }
        
        Assert.Equal(expectedMovesArray.Length, actualMoves.Length);
        actualMoves.Should().BeEquivalentTo( expectedMovesArray );
    }
}