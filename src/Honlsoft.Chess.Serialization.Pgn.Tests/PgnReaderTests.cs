using FluentAssertions;
using Honlsoft.Chess.Serialization.Pgn.Tests.Samples;

namespace Honlsoft.Chess.Serialization.Pgn.Tests;

public class PgnReaderTests {
    
    [Theory()]
    [MemberData(nameof(ValidAttributes))]
    public void ReadTag_Succeeds(string pgnText, PgnTag expectedResult) {
        var serializer = new PgnReader(new PgnTokenizer(pgnText));
        var actualResult = serializer.ReadTag();
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(ValidMoveParts))]
    public void ReadMovePart_Succeeds(string pgnText, PgnMovePart expectedResult) {
        var serializer = new PgnReader(new PgnTokenizer(pgnText));
        var actualResult = serializer.ReadMovePart();
        actualResult.Should().Be(expectedResult);
    }

    [Theory]
    [MemberData(nameof(PgnGames))]
    public void ReadPgnGame(string fileName, int moves) {
        var serializer = new PgnReader(new PgnTokenizer(PgnSample.Read(fileName)));

        var actualResult = serializer.Read();

        actualResult.Should().NotBeNull();
        actualResult.Moves.Should().HaveCount(moves);

    }

    [Fact]
    public void ReadPgnGame_EndingInDraw()
    {
        var serializer = new PgnReader(new PgnTokenizer(PgnSample.Read("game003.pgn")));

        var actualResult = serializer.Read();
    }

    private PgnReader CreateReader(string pgnText) {
        return new PgnReader(new PgnTokenizer(pgnText));
    }

    public static TheoryData<string, PgnMovePart> ValidMoveParts {
        get {
            TheoryData<string, PgnMovePart> data = new();
            data.Add("1.", new PgnMoveNumber(1, 1));
            data.Add("1...", new PgnMoveNumber(1, 3));
            data.Add("e2e4", new PgnMoveText(new SanMove {
                FromFile = SquareFile.e, 
                FromRank = SquareRank.Rank2,
                ToFile = SquareFile.e,
                ToRank = SquareRank.Rank4
            }));
            return data;
        }
    }
    
    public static TheoryData<string, PgnTag> ValidAttributes {
        get {
            TheoryData<string, PgnTag> data = new TheoryData<string, PgnTag>();
            data.Add("[Name \"William Riker\"]", new PgnTag("Name", "William Riker", null));
            data.Add("[Starship  \t  \"Enterprise \\\\ \\\"Class D\\\"\"]", new PgnTag("Starship", "Enterprise \\ \"Class D\"", null));
            return data;
        }
    }

    public static TheoryData<string, int> PgnGames {
        get {
            TheoryData<string, int> data = new();
            data.Add("game001.pgn", 49);
            data.Add("game002.pgn", 63);
            data.Add("game003.pgn", 141);
            data.Add("game004.pgn", 71);
            return data;
        }
    }
}