using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Tests; 

public class MoveCounterTests {
    
    
    [Theory]
    [InlineData(PieceColor.Black)]
    [InlineData(PieceColor.White)]
    public void MoveCounter_ReturnsCorrectNumberOfMoves(PieceColor color) {
        var moveCounter = new MoveCounter(ChessBoard.NewStandardGame(), ChessBoardUtils.GetAllMoveRules(), color);
        Assert.Equal(20, moveCounter.GetTotalMoves());
    }
}