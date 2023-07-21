using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class KnightMoveRuleTests {


    [Fact]
    public void GetPossibleMoves_CenterSquare_ReturnsAllPositions() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Ne4");
        KnightMoveRule rule = new KnightMoveRule();
        var moves = rule.GetPossibleMoves(fakeChessBoard, SquareName.Parse("e4"));

        var expected = ChessBoardUtils.GetSquares("c3", "c5", "d6", "d2", "f2", "f6", "g3", "g5");
        
        moves.Should().HaveCount(8).And.BeEquivalentTo(expected);

    }

    [Fact]
    public void GetPossibleMoves_EmptySquare_NoMoves() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard();
        var rule = new KnightMoveRule();
        var moves = rule.GetPossibleMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Should().HaveCount(0);
    }
    
    
    
    [Fact]
    public void GetPossibleMoves_WrongPiece_NoMoves() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Qe4");
        var rule = new KnightMoveRule();
        var moves = rule.GetPossibleMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Should().HaveCount(0);
    }

    [Fact]
    public void IsApplicable_Knight_True() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Na1");
        var rule = new KnightMoveRule();
        var result = rule.IsApplicable(fakeChessBoard, SquareName.Parse("a1"));
        result.Should().BeTrue();
    }

}