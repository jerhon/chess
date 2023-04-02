namespace Honlsoft.Chess.Tests;

public class SquareNameTests {
    
    [Theory]
    [InlineData("a1", 'a', 1)]
    [InlineData("a8", 'a', 8)]
    [InlineData("g1", 'g', 1)]
    [InlineData("g8", 'g', 8)]
    public void Parse_ReturnsProperValue(string value, char file, int rank) {

        bool result = SquareName.TryParse(value, out var squareName);
        
        Assert.True(result);
        Assert.Equal(file, squareName.File.Name);
        Assert.Equal(rank, squareName.Rank.Number);
    }


    [Theory]
    [InlineData('a', 1, "a1")]
    public void ToString_ReturnsProperValue(char file, int rank, string result) {

        var squareName = new SquareName(new File(file), new Rank(rank));
        
        Assert.Equal(result, squareName.ToString());
    }
}