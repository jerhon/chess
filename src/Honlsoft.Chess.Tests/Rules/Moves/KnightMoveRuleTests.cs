using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class KnightMoveRuleTests {
    
    [Fact]
    public void GetPossibleMoves_CenterSquare_ReturnsAllPositions() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Ne4");
        KnightMoveRule rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("e4"));

        var expected = ChessBoardUtils.CreateCandidateMoves("e4", "c3", "c5", "d6", "d2", "f2", "f6", "g3", "g5");
        
        moves.Should().HaveCount(8).And.BeEquivalentTo(expected);

    }

    [Fact]
    public void GetPossibleMoves_EmptySquare_NoMoves() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard();
        var rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Should().HaveCount(0);
    }
    
    
    [Fact]
    public void GetPossibleMoves_WrongPiece_NoMoves() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Qe4");
        var rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Should().HaveCount(0);
    }

    [Fact]
    public void IsApplicable_Knight_True() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Na1");
        var rule = new KnightMoveRule();
        var result = rule.IsApplicable(fakeChessBoard, SquareName.Parse("a1"));
        result.Should().BeTrue();
    }
    
    [Fact]
    public void GetPossibleMoves_BlockedBySameColor_NoMovesToBlockedLocation() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Ne4", "Ng5");
        var rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Select((m) => m.To).Should().NotContain(SquareName.Parse("g5"));
    }
    
    [Fact]
    public void GetPossibleMoves_CanCaptureEnemy_PossibleMoveToEnemyLocation() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Ne4", "ng5");
        var rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("e4"));
        moves.Select((m) => m.To).Should().Contain(SquareName.Parse("g5"));
    }
    
    [Fact]
    public void GetPossibleMoves_KnightAtEdge_FewerMoves() {
        FakeChessBoard fakeChessBoard = new FakeChessBoard().AddPieces("Nh8");
        var rule = new KnightMoveRule();
        var moves = rule.GetCandidateMoves(fakeChessBoard, SquareName.Parse("h8"));
        var expected = ChessBoardUtils.GetSquares("g6", "f7");
        moves.Select((m) => m.To).Should().HaveCount(2).And.BeEquivalentTo(expected);
    }
}