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
    
    [Theory]
    [MemberData(nameof(FenMoves))]
    public void Move_WithFenAndSanMove_ReturnsProperFen(string startingFen, string move, string endingFen)
    {
        FenSerializer fenSerializer = new FenSerializer();
        SanSerializer sanSerializer = new SanSerializer();
        
        ChessGame chessGame = FromFen(startingFen);
        var moveResult = chessGame.Move(sanSerializer.Deserialize(move));
        moveResult.Should().Be(MoveResult.ValidMove);
        fenSerializer.Serialize(chessGame.CurrentPosition).Should().Be(endingFen);
    }


    public static TheoryData<string, string> InvalidMoves() {
        TheoryData<string, string> invalidMoves = new();
        invalidMoves.Add("a1", "a8");
        invalidMoves.Add("b1", "c4");
        invalidMoves.Add("d1", "d6");
        invalidMoves.Add("e1", "f1");
        return invalidMoves;
    }

    public static TheoryData<string, string, string> FenMoves()
    {
        TheoryData<string, string, string> fenMoves = new();
        fenMoves.Add("rnb1kb1r/ppp1pppp/8/8/3Pq3/8/PPP1BPPP/R1BQK1NR w - - 0 1", "Be3", "rnb1kb1r/ppp1pppp/8/8/3Pq3/4B3/PPP1BPPP/R2QK1NR b - - 1 1");
        fenMoves.Add("rnb1kb1r/ppp1pppp/8/8/3Pq3/8/PPP1BPPP/R1BQK1NR b KQkq - 2 5", "e6", "rnb1kb1r/ppp2ppp/4p3/8/3Pq3/8/PPP1BPPP/R1BQK1NR w KQkq - 0 6");
        fenMoves.Add("r1b1kb1r/p1p2ppp/2n1p3/1p6/3Pq3/4BN2/PPP1BPPP/R2QK2R w KQkq b6 0 8", "O-O", "r1b1kb1r/p1p2ppp/2n1p3/1p6/3Pq3/4BN2/PPP1BPPP/R2Q1RK1 b kq - 1 8");
        return fenMoves;
    }
    
    
}