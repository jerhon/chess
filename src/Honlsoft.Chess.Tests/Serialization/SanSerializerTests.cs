using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization;

public class SanSerializerTests {


    [Theory]
    [MemberData(nameof(GetAlgebraicNotations))]
    public void Deserialize_WithValidExpression_ShouldMatch(string rawSan, San expectedSanMove) {
        SanSerializer serializer = new SanSerializer();
        San actualSanMove = serializer.Deserialize(rawSan);

        actualSanMove.Should().Be(expectedSanMove);
    }


    [Theory]
    [MemberData(nameof(GetAlgebraicNotations))]
    public void Serialize_WithValidExpression_ShouldMatch(string expectRawSan, San expectedSanMove) {
        SanSerializer serializer = new SanSerializer();
        string actualRawSan = serializer.Serialize(expectedSanMove);

        actualRawSan.Should().Be(expectRawSan);
    }


    public static TheoryData<string, San> GetAlgebraicNotations() {
        TheoryData<string, San> data = new();
        data.Add("b4", new SanMove { ToFile = SquareFile.b, ToRank = SquareRank.Rank4 });
        data.Add("Nc6", new SanMove { ToFile = SquareFile.c, ToRank = SquareRank.Rank6, FromPiece = PieceType.Knight });
        data.Add("Qd8+", new SanMove{ ToFile = SquareFile.d, ToRank = SquareRank.Rank8, FromPiece = PieceType.Queen, Check = SanCheckType.Check });
        data.Add("Kd8#", new SanMove{ ToFile = SquareFile.d, ToRank = SquareRank.Rank8, FromPiece = PieceType.King, Check = SanCheckType.Checkmate });
        data.Add("Ba1c4", new SanMove{ FromFile = SquareFile.a, FromRank = SquareRank.Rank1, FromPiece = PieceType.Bishop, ToFile = SquareFile.c, ToRank = SquareRank.Rank4});
        data.Add("Rh1xh8", new SanMove{ FromFile = SquareFile.h, FromRank = SquareRank.Rank1, FromPiece = PieceType.Rook, ToFile = SquareFile.h, ToRank = SquareRank.Rank8, Capture = true});
        data.Add("O-O", new SanCastle{ Side=CastlingSide.Kingside });
        data.Add("O-O-O", new SanCastle{ Side = CastlingSide.Queenside });
        data.Add("O-O+", new SanCastle{ Side = CastlingSide.Kingside, Check = SanCheckType.Check });
        data.Add("O-O-O#", new SanCastle{ Side = CastlingSide.Queenside, Check = SanCheckType.Checkmate });
        
        return data;
    }
}