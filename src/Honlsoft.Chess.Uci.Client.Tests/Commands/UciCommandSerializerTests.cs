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
    public void DeserializeCommand_SimpleCommands(string commandString) {

        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);

        command.Should().BeEquivalentTo(new UciCommand(commandString, null));
    }


    [Theory]
    [InlineData("debug on", "on")]
    [InlineData("debug off", "off")]
    public void DeserializeCommand_debug(string commandString, string parameter) {
        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);
        command.Should().BeEquivalentTo(new UciCommand("debug", new [] { new UciParameter(null, parameter)}));
    }

    [Theory]
    [InlineData("id author Jean Luc Picard", "author", "Jean Luc Picard")]
    [InlineData("id name Honlsoft Chess", "name", "Honlsoft Chess")]
    public void DeserializeCommand_id(string commandString, string parameterKey, string parameterValue) {
        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);
        command.Should().BeEquivalentTo(new UciCommand("id", new UciParameter[] { new UciParameter(parameterKey, parameterValue) }));
    }

    [Theory]
    [MemberData(nameof(OptionCommands))]
    public void DeserializeCommand_option(string commandString, UciCommand expected) {
        var serializer = NewObj();
        var command = serializer.DeserializeCommand(commandString);

        command.Should().BeEquivalentTo(expected);
    }
    
    
    
    public static TheoryData<string, UciCommand> OptionCommands {
        get {
            TheoryData<string, UciCommand> data = new TheoryData<string, UciCommand>();

            // Option for a Debug Log File
            UciCommand command = new UciCommand("option", new[] {
                new UciParameter("name", "Debug Log File"),
                new UciParameter("type", "string"),
                new UciParameter("default", "")
            });
            data.Add("option name Debug Log File type string default", command);

            return data;
        }
    }

}