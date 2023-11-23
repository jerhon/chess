using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests; 

public class ChessGameTests {


    public ChessGame FromFen(string fenString) {
        FenSerializer serializer = new FenSerializer();
        IChessPosition initialPosition = serializer.Deserialize(fenString);

        ChessGameFactory factory = new ChessGameFactory();
        return factory.CreateGameFromPosition(initialPosition);
    }
    
    [Theory]
    [MemberData(nameof(InvalidMoves))]
    public void Move_WithInvalidMove_ReturnsInvalidMove(string from, string to) {

        ChessGame chessGame = FromFen("rnbqkbnr/2p2p2/1p4p1/p2pp2p/P2PP2P/1P4P1/2P2P2/RNBQKBNR w KQkq - 0 1");

        MoveResult moveResult = chessGame.Move(SquareName.Parse(from), SquareName.Parse(to), null);
        
        Assert.Equal(MoveResult.NotALegalMove, moveResult);
    }


    public static TheoryData<string, string> InvalidMoves() {
        TheoryData<string, string> invalidMoves = new();
        invalidMoves.Add("a1", "a8");
        invalidMoves.Add("b1", "c4");
        invalidMoves.Add("d1", "d6");
        invalidMoves.Add("e1", "f1");
        return invalidMoves;
    }
    
    
}