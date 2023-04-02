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
}