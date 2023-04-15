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
        Assert.Equal(5, newObj!.Number);
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

    [Fact]
    public void ToRange_Positive_TwoNumbers() {
        var range = Rank.Rank4.ToRange(2).ToArray();
        Assert.Equal(new[] { Rank.Rank5, Rank.Rank6 }, range);
    }

    [Fact]
    public void ToRange_Negative_TwoNumbers() {
        var range = Rank.Rank4.ToRange(-2).ToArray();
        Assert.Equal(new[] { Rank.Rank3, Rank.Rank2 }, range);
    }

    [Fact]
    public void ToEnd_FromFour() {
        var range = Rank.Rank4.ToEnd();
        Assert.Equal(new[] { Rank.Rank5, Rank.Rank6, Rank.Rank7, Rank.Rank8 }, range);
    }

    
    [Fact]
    public void ToStart_FromFour() {
        var range = Rank.Rank4.ToStart();
        Assert.Equal(new[] { Rank.Rank3, Rank.Rank2, Rank.Rank1 }, range);
    }

    [Fact]
    public void ToEnd_AllToEnd() {
        var range = Rank.Rank1.ToEnd(true);
        Assert.Equal(Rank.AllRanks, range);
    }

    [Fact]
    public void ToStart_AllFromEnd() {
        var range = Rank.Rank8.ToStart(true);
        Assert.Equal(Rank.AllRanks.Reverse(), range);
    }
    
}