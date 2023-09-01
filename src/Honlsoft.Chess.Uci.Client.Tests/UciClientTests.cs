namespace Honlsoft.Chess.Uci.Client.Tests; 

public class UciClientTests {


    [Fact]
    public async Task UciAsync_WithStockfishResponses_ReturnsCorrectly() {

        FakeUciInterface fakeUciInterface = new FakeUciInterface();
        fakeUciInterface.ReadResponses("Output\\Stockfish\\uci.txt");
        
        UciClient client = new UciClient(fakeUciInterface);
        await client.UciAsync(CancellationToken.None);

        client.Name.Equals("Stockfish 16");
        client.Author.Equals("the Stockfish developers (see AUTHORS file)");

        client.Options.Count.Should().Be(21);
    }
    
}