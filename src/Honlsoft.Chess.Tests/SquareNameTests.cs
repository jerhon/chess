namespace Honlsoft.Chess.Tests;

public class SquareNameTests {
    
    [Theory]
    [InlineData("a1", 'a', 1)]
    [InlineData("a8", 'a', 8)]
    [InlineData("g1", 'g', 1)]
    [InlineData("g8", 'g', 8)]
    public void Parse_ReturnsProperValue(string value, char file, int rank) {

        var squareName = SquareName.Parse(value, null);
        
        Assert.Equal(file, squareName.SquareFile.Name);
        Assert.Equal(rank, squareName.SquareRank.Number);
    }


    [Theory]
    [InlineData('a', 1, "a1")]
    public void ToString_ReturnsProperValue(char file, int rank, string result) {

        var squareName = new SquareName(new SquareFile(file), new SquareRank(rank));
        
        Assert.Equal(result, squareName.ToString());
    }

    [Theory]
    [InlineData("a0")]
    [InlineData("a9")]
    [InlineData("i3")]
    [InlineData("$2")]
    [InlineData("z9")]
    [InlineData("a30")]
    public void Parse_Invalid_ThrowsException(string invalidSquareName) {
        Assert.Throws<FormatException>(() => {
            SquareName.Parse(invalidSquareName);
        });
    }
    
}