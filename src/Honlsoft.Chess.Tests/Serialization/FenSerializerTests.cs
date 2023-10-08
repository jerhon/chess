using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization; 

public class FenSerializerTests {


    [Fact]
    public void FenSerializer_ChessBoardStandardGame() {
        FenSerializer serialzer = new FenSerializer();
        string standardFen = serialzer.Serialize(ChessPositionBuilder.StandardGame);

        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", standardFen);
    }

    [Fact]
    public void FenSerializer_ChessBoard_WithGaps() {
        var squareName = SquareName.Parse("g8");
        
        ChessPositionBuilder chessPosition = new ChessPositionBuilder();
        chessPosition.RemovePiece(squareName);
        
        FenSerializer serializer = new();
        string actual = serializer.Serialize(chessPosition);
    }
    
}