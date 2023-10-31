namespace Honlsoft.Chess.Tests; 

public class FileTests {


    [Theory]
    [InlineData('a', "a")]
    [InlineData('b', "b")]
    [InlineData('c', "c")]
    public void ToString_Succeeds(char fileParameter, string result) {
        var file = new SquareFile(fileParameter);
        Assert.Equal(result, file.ToString());
    }


    [Fact]
    public void IsFirst_True_WhenFirstFile() {
        Assert.True(SquareFile.a.IsFirst);
    }

    [Fact]
    public void IsFirst_False_WhenNotFirstFile() {
        Assert.False(SquareFile.b.IsFirst);
        Assert.False(SquareFile.c.IsFirst);
        Assert.False(SquareFile.d.IsFirst);
        Assert.False(SquareFile.e.IsFirst);
        Assert.False(SquareFile.f.IsFirst);
        Assert.False(SquareFile.g.IsFirst);
        Assert.False(SquareFile.h.IsFirst);
    }

    [Fact]
    public void Add_d_When3AddedToa() {
        var objUnderTest = SquareFile.a;
        var fileD = objUnderTest.Add(3);
        Assert.NotNull(fileD);
        Assert.Equal('d', fileD.Name);
        Assert.NotEqual(fileD, objUnderTest);
    }

    [Fact]
    public void Add_null_WhenOffBoard() {
        var objUnderTest = SquareFile.h;
        var offBoard1 = objUnderTest.Add(1);
        Assert.Null(offBoard1);
        
        objUnderTest = SquareFile.a;
        var offBoard2 = objUnderTest.Add(-1);
        Assert.Null(offBoard2);
    }

    [Fact]
    public void ToRange_TwoValues_AfgerC() {
        var files = SquareFile.c.ToRange(2, false).ToArray();
        Assert.Equal(new[] { SquareFile.d, SquareFile.e }, files);
    }

    [Fact]
    public void ToRange_TwoValues_BeforeE() {
        var files = SquareFile.e.ToRange(-2, false).ToArray();
        Assert.Equal(new [] { SquareFile.d, SquareFile.c }, files);
    }
}