using System.Runtime.InteropServices;
using FluentAssertions;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client.Tests.Commands; 

public class UciCommandSerializerTests {

    private UciCommandSerializer NewObj() {
        return new UciCommandSerializer();
    }
    
    
    [Theory]
    [InlineData("uci")]
    [InlineData("uciok")]
    [InlineData("isready")]
    [InlineData("ucinewgame")]
    [InlineData("stop")]
    [InlineData("ponderhit")]
    [InlineData("quit")]
    [InlineData("readyok")]
    public void ParseSimpleCommands(string commandString) {

        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);

        command.Should().BeEquivalentTo(new UciCommand(commandString, null));
    }


    [Theory]
    [InlineData("debug on", "on")]
    [InlineData("debug off", "off")]
    public void ParseDebug(string commandString, string parameter) {
        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);
        command.Should().BeEquivalentTo(new UciCommand("debug", new [] { new UciParameter() { Value = parameter } }));
    }

    [Theory]
    [InlineData("id author Jean Luc Picard", "author", "Jean Luc Picard")]
    [InlineData("id name Honlsoft Chess", "name", "Honlsoft Chess")]
    public void ParseIdCommand(string commandString, string parameterKey, string parameterValue) {
        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);
        command.Should().BeEquivalentTo(new UciCommand("id", new UciParameter[] { new UciParameter() { Key = parameterKey, Value = parameterValue }}));
    }
}