namespace Honlsoft.Chess.Tests; 

public class RankTests {

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    public void ToString_ReturnsSuccessfully(int rank, string result) {
        var objUnderTest = new Rank(rank);
        Assert.Equal(result, objUnderTest.ToString());
    }

    
    [Fact]
    public void Add_AddsValueCorrectly() {
        var objUnderTest = Rank.Rank2;

        var newObj = objUnderTest.Add(3);
        Assert.NotEqual(objUnderTest, newObj);
        Assert.Equal(5, newObj.Number);
    }

    [Fact]
    public void Add_ReturnsNullWhenOffBoard() {
        var objUnderTest = Rank.Rank8;
        var newObj = objUnderTest.Add(1);
        Assert.Null(newObj);
    }

    [Fact]
    public void Add_ReturnsNullWhenOffOppositeSide() {
        var objUnderTest = Rank.Rank1;
        var newObj = objUnderTest.Add(-1);
        Assert.Null(newObj);
    }
}