using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Rules; 

public class RuleTest {


    public static void TestRuleCandidates(string boardSquareSetup, string fromSquareName, string candateToSquareNames, IMoveRule rule) {
        SimpleSerializer serializer = new SimpleSerializer();
        var chessBoard = serializer.Deserialize(boardSquareSetup);
        
        var actualMoves = rule.GetCandidateMoves(chessBoard, SquareName.Parse(fromSquareName));

        SimpleMove[] expectedMovesArray = [];
        if (!string.IsNullOrEmpty(candateToSquareNames)) {
            expectedMovesArray = candateToSquareNames.Split(":").Select((m) => new SimpleMove(SquareName.Parse(fromSquareName), SquareName.Parse(m))).ToArray();
        }
        
        Assert.Equal(expectedMovesArray.Length, actualMoves.Length);
        actualMoves.Should().BeEquivalentTo( expectedMovesArray );
    }
}