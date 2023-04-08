namespace Honlsoft.Chess.Tests; 

public class FileTests {


    [Theory]
    [InlineData('a', "a")]
    [InlineData('b', "b")]
    [InlineData('c', "c")]
    public void ToString_Succeeds(char fileParameter, string result) {
        var file = new File(fileParameter);
        Assert.Equal(result, file.ToString());
    }


    [Fact]
    public void IsFirst_True_WhenFirstFile() {
        Assert.True(File.a.IsFirst);
    }

    [Fact]
    public void IsFirst_False_WhenNotFirstFile() {
        Assert.False(File.b.IsFirst);
        Assert.False(File.c.IsFirst);
        Assert.False(File.d.IsFirst);
        Assert.False(File.e.IsFirst);
        Assert.False(File.f.IsFirst);
        Assert.False(File.g.IsFirst);
        Assert.False(File.h.IsFirst);
    }

    [Fact]
    public void Add_d_When3AddedToa() {
        var objUnderTest = File.a;
        var fileD = objUnderTest.Add(3);
        Assert.NotNull(fileD);
        Assert.Equal('d', fileD.Name);
        Assert.NotEqual(fileD, objUnderTest);
    }

    [Fact]
    public void Add_null_WhenOffBoard() {
        var objUnderTest = File.h;
        var offBoard1 = objUnderTest.Add(1);
        Assert.Null(offBoard1);
        
        objUnderTest = File.a;
        var offBoard2 = objUnderTest.Add(-1);
        Assert.Null(offBoard2);
    }
}