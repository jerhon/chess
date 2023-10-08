using FluentAssertions;

namespace Honlsoft.Chess.Serialization.Pgn.Tests;

public class PgnSerializerTests {
    
    [Theory()]
    [MemberData(nameof(ValidAttributes))]
    public void ParseAttribute_Succeeds(string pgnText, PgnAttribute expectedResult) {
        PgnSerializer serialzer = new PgnSerializer();

        var actualResult = serialzer.ParseAttribute(pgnText);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    public static TheoryData<string, PgnAttribute> ValidAttributes {
        get {
            TheoryData<string, PgnAttribute> data = new TheoryData<string, PgnAttribute>();
            data.Add("[Name \"William Riker\"]", new PgnAttribute("Name", "William Riker", string.Empty));
            data.Add("[Starship  \t  \"Enterprise \\\"Class D\\\"\"]", new PgnAttribute("Starship", "Enterprise \"Class D\"", string.Empty));
            return data;
        }
    }
    
}