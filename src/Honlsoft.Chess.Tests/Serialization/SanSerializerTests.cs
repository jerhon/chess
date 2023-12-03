using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests.Serialization;

public class SanSerializerTests {


    [Theory]
    [MemberData(nameof(GetAlgebraicNotations))]
    public void Deserialize_WithValidExpression_ShouldMatch(string rawSan, San expectedSan) {
        SanSerializer serializer = new SanSerializer();
        San actualSan = serializer.Deserialize(rawSan);

        actualSan.Should().BeEquivalentTo(expectedSan);
    }


    [Theory]
    [MemberData(nameof(GetAlgebraicNotations))]
    public void Serialize_WithValidExpression_ShouldMatch(string expectRawSan, San expectedSan) {
        SanSerializer serializer = new SanSerializer();
        string actualRawSan = serializer.SerializeSan(expectedSan);

        actualRawSan.Equals(expectRawSan);
    }


    public static TheoryData<string, San> GetAlgebraicNotations() {
        TheoryData<string, San> data = new();
        data.Add("b4", new San { FromFile = SquareFile.b, FromRank = SquareRank.Rank4 });
        data.Add("Nc6", new San { FromFile = SquareFile.c, FromRank = SquareRank.Rank6, FromPiece = PieceType.Knight });
        data.Add("Qd8+", new San{ FromFile = SquareFile.d, FromRank = SquareRank.Rank8, FromPiece = PieceType.Queen, Check = SanCheckType.Check });
        data.Add("Kd8#", new San{ FromFile = SquareFile.d, FromRank = SquareRank.Rank8, FromPiece = PieceType.King, Check = SanCheckType.Checkmate });
        data.Add("Ba1c4", new San{ FromFile = SquareFile.a, FromRank = SquareRank.Rank1, FromPiece = PieceType.Bishop, ToFile = SquareFile.c, ToRank = SquareRank.Rank4});
        data.Add("Rh1xh8", new San{ FromFile = SquareFile.h, FromRank = SquareRank.Rank1, FromPiece = PieceType.Rook, ToFile = SquareFile.h, ToRank = SquareRank.Rank8, Capture = true});
        return data;
    }
}