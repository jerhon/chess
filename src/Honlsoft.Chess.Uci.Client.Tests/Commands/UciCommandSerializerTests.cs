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

    [Theory]
    [MemberData(nameof(CommandsToString))]
    public void SerializeCommand(UciCommand command, string expected) {
        UciCommandSerializer serializer = new UciCommandSerializer();
        string actual = serializer.SerializeCommand(command);

        actual.Should().Be(expected);
    }

    [Fact]
    public void Deserialize_bestmove() {
        var serializer = NewObj();

        var actual = serializer.DeserializeCommand("bestmove e2e4 ponder e7e5");

        UciCommand expected = new UciCommand("bestmove",
            [
                new UciParameter(null, "e2e4"),
                new UciParameter("ponder", "e7e5")
            ]
        );

        actual.Should().BeEquivalentTo(expected);
    }
    
    
    
    public static TheoryData<string, UciCommand> OptionCommands {
        get {
            TheoryData<string, UciCommand> data = new TheoryData<string, UciCommand>();

            // Option for a Debug Log File
            UciCommand command = new UciCommand("option", [
                new UciParameter("name", "Debug Log File"),
                new UciParameter("type", "string"),
                new UciParameter("default", "")
            ]);
            data.Add("option name Debug Log File type string default", command);

            return data;
        }
    }

    public static TheoryData<UciCommand, string> CommandsToString {
        get {
            TheoryData<UciCommand, string> data = new TheoryData<UciCommand, string>();
            data.Add(new UciCommand("position", new [] { new UciParameter("startpos", null), new UciParameter("moves", "e2e4 e7e5") }),
                "position startpos moves e2e4 e7e5\n");

            data.Add(new UciCommand("isready"), "isready\n");
            return data;
        }  
    }

}