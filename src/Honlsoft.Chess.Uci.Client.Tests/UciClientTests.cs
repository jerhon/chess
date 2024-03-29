﻿using System.ComponentModel.DataAnnotations.Schema;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client.Tests; 

public class UciClientTests {
    
    [Fact]
    public async Task UciAsync_WithStockfishResponses_ReturnsCorrectly() {

        FakeUciInterface fakeUciInterface = new FakeUciInterface();
        fakeUciInterface.ReadResponses("Output\\Stockfish\\uci.txt");
        
        UciClient client = new UciClient(fakeUciInterface);
        await client.UciAsync(CancellationToken.None);

        client.Name.Should().Be("Stockfish 16");
        client.Author.Should().Be("the Stockfish developers (see AUTHORS file)");

        client.Options.Count.Should().Be(21);
    }
    
    [Fact]
    public async Task IsReadyAsync_ReturnsCorrectly() {
        FakeUciInterface fakeUciInterface = new FakeUciInterface();
        fakeUciInterface.AddResponses(null, null, new UciCommand("readyok"));

        UciClient client = new UciClient(fakeUciInterface);
        await client.IsReadyAsync(CancellationToken.None);
    }
    
    [Fact]
    public async Task SetStartPositionAsync_ReturnsCorrectly() {
        FakeUciInterface fakeUciInterface = new FakeUciInterface();

        SanSerializer serializer = new SanSerializer();
        var move1 = serializer.Deserialize("e2e4");
        var move2 = serializer.Deserialize("e7e5");
        
        UciClient client = new UciClient(fakeUciInterface);
        await client.SetStartingPositionAsync([move1, move2], CancellationToken.None);

        fakeUciInterface.Requests.Should().HaveCount(1)
            .And.BeEquivalentTo(new[] {
                new UciCommand("position", new[] { new UciParameter("startpos", null), new UciParameter("moves", "e2e4 e7e5")})
            });
    }
}