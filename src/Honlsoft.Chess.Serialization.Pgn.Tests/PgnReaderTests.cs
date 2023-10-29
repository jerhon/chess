using FluentAssertions;

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

    public static TheoryData<string, PgnMovePart> ValidMoveParts {
        get {
            TheoryData<string, PgnMovePart> data = new();
            data.Add("1.", new PgnMoveNumber(1, 1));
            data.Add("1...", new PgnMoveNumber(1, 3));
            data.Add("e2e4", new PgnMoveText("e2e4"));
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
}