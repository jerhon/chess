using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization; 


using CastlingRights = (CastlingSide Side, PieceColor Color);
using CastlingTestData = TheoryData<string, (CastlingSide Side, PieceColor Color)[], (CastlingSide Side, PieceColor Color)[]>;


public class FenSerializerTests {

 
    [Fact]
    public void FenSerializer_Serialize_ChessBoardStandardGame() {
        FenSerializer serialzer = new FenSerializer();
        string standardFen = serialzer.Serialize(ChessPositionBuilder.StandardGame);

        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0", standardFen);
    }

    [Fact]
    public void FenSerializer_ChessBoard_WithGaps() {
        var squareName = SquareName.Parse("g8");
        
        ChessPositionBuilder chessPosition = new ChessPositionBuilder();
        chessPosition.RemovePiece(squareName);
        
        FenSerializer serializer = new();
        string actual = serializer.Serialize(chessPosition);
    }

    [Fact]
    public void Deserialize_StandardGame() {
        FenSerializer serializer = new FenSerializer();
        var chessPosition = serializer.Deserialize("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq b2 2 20");

        // check squares that have no values
        var emptyFiles = Enumerable.Range(3, 3)
            .SelectMany((i) => SquareFile.AllFiles.Select((f) => SquareName.From(f.Name, i)))
            .Select((sn) => new Square(sn, null));
        
        var squares = new string[] {
            //black pieces
            "ra8",
            "nb8",
            "bc8",
            "qd8",
            "ke8",
            "bf8",
            "ng8",
            "rh8",
            // black pawns
            "pa7",
            "pb7",
            "pc7",
            "pd7",
            "pe7",
            "pf7",
            "pg7",
            "ph7",
            //white pawns
            "Pa2",
            "Pb2",
            "Pc2",
            "Pd2",
            "Pe2",
            "Pf2",
            "Pg2",
            "Ph2",
            // white pieces
            "Ra1",
            "Nb1",
            "Bc1",
            "Qd1",
            "Ke1",
            "Bf1",
            "Ng1",
            "Rh1"
        }.Select((ex) => Square.Parse(ex)).Concat(emptyFiles);
        
        Assert.All(squares, (s) => Assert.Equal(s, chessPosition.GetSquare(s.Name)));
        Assert.Equal(PieceColor.White, chessPosition.PlayerToMove);
        Assert.Equal(SquareName.Parse("b2"), chessPosition.EnPassantTarget);
        Assert.Equal(2, chessPosition.HalfMoves);
        Assert.Equal(20, chessPosition.FullMoves);
    }

    [Theory]
    [MemberData(nameof(CastlingRights))]
    public void DeserializeCastlingRights_VariousValues_Match(string serialization, CastlingRights[] castling, CastlingRights[] notCastling) {
        ChessPositionBuilder builder = new();
        FenSerializer serializer = new();
        serializer.DeserializeCastlingRights(builder, serialization);

        foreach (var rights in castling) {
            Assert.True(builder.CanCastle(rights.Color, rights.Side));
        }
        foreach (var rights in notCastling) {
            Assert.False(builder.CanCastle(rights.Color, rights.Side));
        }
    }
    

    public static TheoryData<string, CastlingRights[], CastlingRights[]> CastlingRights() {
        TheoryData<string, CastlingRights[], CastlingRights[]> rights = new();
        rights.Add("KQkq", [
            (CastlingSide.Kingside, PieceColor.White), 
            (CastlingSide.Queenside, PieceColor.White), 
            (CastlingSide.Kingside, PieceColor.Black), 
            (CastlingSide.Queenside, PieceColor.Black)],
            []);
        
        
        rights.Add("KQ", [
            (CastlingSide.Kingside, PieceColor.White), 
            (CastlingSide.Queenside, PieceColor.White)], 
            [(CastlingSide.Kingside, PieceColor.Black), 
            (CastlingSide.Queenside, PieceColor.Black)]
            );
        
        rights.Add("Kk", [
                (CastlingSide.Kingside, PieceColor.White), 
                (CastlingSide.Kingside, PieceColor.Black)], 
            [(CastlingSide.Queenside, PieceColor.White), 
                (CastlingSide.Queenside, PieceColor.Black)]
        );
        return rights;
    }
    
}