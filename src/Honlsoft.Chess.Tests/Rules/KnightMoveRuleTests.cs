using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules; 

public class KnightMoveRuleTests {


    [Fact]
    public void GetCandidateMoves_CenterSquare_ReturnsAllPositions() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPiece("Ke4");
        KnightMoveRule rule = new KnightMoveRule();
        var moves = rule.GetPossibleMoves(fakeChessBoard, SquareName.Parse("e4"));

        var expected = ChessBoardUtils.GetSquares("c3", "c5", "d6", "d2", "f2", "f6", "g3", "g5");
        
        moves.Should().HaveCount(8).And.BeEquivalentTo(expected);

    }

    [Fact]
    public void GetCandidateMoves_NoKnight_() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard();
        var rule = new KnightMoveRule();
        var moves = rule.GetPossibleMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Should().HaveCount(0);
    }
    
}