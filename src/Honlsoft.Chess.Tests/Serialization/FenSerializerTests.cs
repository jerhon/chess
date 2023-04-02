using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization; 

public class FenSerializerTests {


    [Fact]
    public void FenSerializer_ChessBoardStandardGame_Success() {
        FenSerializer serialzer = new FenSerializer();
        string standardFen = serialzer.Serialize(ChessBoard.StandardGame);

        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", standardFen);
    }
    
}