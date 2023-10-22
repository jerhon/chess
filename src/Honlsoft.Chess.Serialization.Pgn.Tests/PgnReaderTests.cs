using FluentAssertions;

namespace Honlsoft.Chess.Serialization.Pgn.Tests;

public class PgnReaderTests {
    
    /*
    [Theory()]
    [MemberData(nameof(ValidAttributes))]
    public void ParseAttribute_Succeeds(string pgnText, PgnTag expectedResult) {
        var serializer = new PgnSerializer();
        var actualResult = serializer.ParseAttribute(pgnText);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    public static TheoryData<string, PgnTag> ValidAttributes {
        get {
            TheoryData<string, PgnTag> data = new TheoryData<string, PgnTag>();
            data.Add("[Name \"William Riker\"]", new PgnTag("Name", "William Riker", string.Empty));
            data.Add("[Starship  \t  \"Enterprise \\\\ \\\"Class D\\\"\"]", new PgnTag("Starship", "Enterprise \\ \"Class D\"", string.Empty));
            return data;
        }
    }
    */
}