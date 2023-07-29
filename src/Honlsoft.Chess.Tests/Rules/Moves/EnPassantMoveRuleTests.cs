using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests.Rules.Moves; 

public class EnPassantMoveRuleTests {

    [Fact]
    public void IsApplicable_True_ForPawnsAndEnPassantBoards() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.EnPassantTarget = SquareName.Parse("b6");
        chessBoard.AddPieces("Pc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeTrue();
    }

    [Fact]
    void IsApplicable_False_WhenNotAPawn() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.EnPassantTarget = SquareName.Parse("b6");
        chessBoard.AddPieces("Nc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsApplicable_False_NoEnPassant() {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.AddPieces("Pc5");

        var result = rule.IsApplicable(chessBoard, SquareName.Parse("c5"));

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Pc4", "b5", "b4")]
    [InlineData("Pa4", "b5", "b4")]
    [InlineData("pa6", "b5", "b6")]
    [InlineData("pc6", "b5", "b6")]
    public void GetPossibleMoves_ReturnsEnpassantSquare(string fromSquareNotation, string toSquareNotation, string enPassantCaptureNotation) {
        var rule = new EnPassantRule();

        var chessBoard = new FakeChessBoard();
        chessBoard.AddPieces(fromSquareNotation);
        chessBoard.EnPassantTarget = SquareName.Parse(toSquareNotation);

        var fromSquare = SquareName.Parse(fromSquareNotation.Substring(1));
        var candidateMoves = rule.GetCandidateMoves(chessBoard, fromSquare);
        candidateMoves.Should().BeEquivalentTo(new[] {
            new ChessMove(
                From: fromSquare,
                To: SquareName.Parse(toSquareNotation)
            )
            {
                EnPassantCapture = SquareName.Parse(enPassantCaptureNotation) 
            }
        });

    }
}