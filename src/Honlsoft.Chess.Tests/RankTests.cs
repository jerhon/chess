namespace Honlsoft.Chess.Tests; 

public class RankTests {

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    public void ToString_ReturnsSuccessfully(int rank, string result) {
        var objUnderTest = new Rank(rank);
        Assert.Equal(result, objUnderTest.ToString());
    }
}