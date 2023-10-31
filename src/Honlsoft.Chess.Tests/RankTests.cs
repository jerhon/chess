namespace Honlsoft.Chess.Tests; 

public class RankTests {

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    public void ToString_ReturnsSuccessfully(int rank, string result) {
        var objUnderTest = new SquareRank(rank);
        Assert.Equal(result, objUnderTest.ToString());
    }
    
    [Fact]
    public void Add_AddsValueCorrectly() {
        var objUnderTest = SquareRank.Rank2;

        var newObj = objUnderTest.Add(3);
        Assert.NotEqual(objUnderTest, newObj);
        Assert.Equal(5, newObj!.Number);
    }

    [Fact]
    public void Add_ReturnsNullWhenOffBoard() {
        var objUnderTest = SquareRank.Rank8;
        var newObj = objUnderTest.Add(1);
        Assert.Null(newObj);
    }

    [Fact]
    public void Add_ReturnsNullWhenOffOppositeSide() {
        var objUnderTest = SquareRank.Rank1;
        var newObj = objUnderTest.Add(-1);
        Assert.Null(newObj);
    }

    [Fact]
    public void ToRange_Positive_TwoNumbers() {
        var range = SquareRank.Rank4.ToRange(2).ToArray();
        Assert.Equal(new[] { SquareRank.Rank5, SquareRank.Rank6 }, range);
    }

    [Fact]
    public void ToRange_Negative_TwoNumbers() {
        var range = SquareRank.Rank4.ToRange(-2).ToArray();
        Assert.Equal(new[] { SquareRank.Rank3, SquareRank.Rank2 }, range);
    }

    [Fact]
    public void ToEnd_FromFour() {
        var range = SquareRank.Rank4.ToEnd();
        Assert.Equal(new[] { SquareRank.Rank5, SquareRank.Rank6, SquareRank.Rank7, SquareRank.Rank8 }, range);
    }

    
    [Fact]
    public void ToStart_FromFour() {
        var range = SquareRank.Rank4.ToStart();
        Assert.Equal(new[] { SquareRank.Rank3, SquareRank.Rank2, SquareRank.Rank1 }, range);
    }

    [Fact]
    public void ToEnd_AllToEnd() {
        var range = SquareRank.Rank1.ToEnd(true);
        Assert.Equal(SquareRank.AllRanks, range);
    }

    [Fact]
    public void ToStart_AllFromEnd() {
        var range = SquareRank.Rank8.ToStart(true);
        Assert.Equal(SquareRank.AllRanks.Reverse(), range);
    }

    [Theory]
    [InlineData(1, 2, 1)]
    [InlineData(1, 8, 7)]
    [InlineData(1, 1, 0)]
    [InlineData(4, 3, -1)]
    [InlineData(8, 1, -7)]
    public void Distance_CalculatesProperly(int rank1, int rank2, int expectedDistance) {
        var rank = new SquareRank(rank1);
        var other = new SquareRank(rank2);
        var result = rank.Distance(other);
        
        result.Should().Be(expectedDistance);
    }
}