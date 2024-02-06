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
        fenMoves.Add("r1b1kbnr/1pp2ppp/p1p5/8/8/5N2/PPPPqPPP/RNB1K2R w KQkq - 0 7", "Kxe2",
            "r1b1kbnr/1pp2ppp/p1p5/8/8/5N2/PPPPKPPP/RNB4R b kq - 0 7");
        fenMoves.Add("1k6/1pp5/p7/r2b4/2K5/8/8/8 w - - 0 43", "Kb4", "1k6/1pp5/p7/r2b4/1K6/8/8/8 b - - 1 43");
        fenMoves.Add("rnbqkbnr/pp2pppp/8/1B1p4/8/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 3", "Bd7", "rn1qkbnr/pp1bpppp/8/1B1p4/8/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 4");
        fenMoves.Add("1k6/1p6/p7/1rpb4/K7/8/8/8 w - c6 0 45", "Ka3", "1k6/1p6/p7/1rpb4/8/K7/8/8 b - - 1 45");
        return fenMoves;
    }
    
    
}