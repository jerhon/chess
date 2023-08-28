using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

public class UciClient {
    private readonly IUciInputOutput _inputOutput;

    public Dictionary<string, string> Options { get; set; }

    public UciClient(IUciInputOutput inputOutput) {
        _inputOutput = inputOutput;
    }
    
    public string Name { get; private set; }
    public string Authors { get; private set; }

    public async Task StartEngineAsync(CancellationToken cancellationToken) {
        // Start it with the UCI command
        await _inputOutput.SendCommandAsync(new UciCommand("uci"), cancellationToken);
        
        // Go through and process each response
        UciCommand? command = null;
        while (command is not { Command: "uciok" }) {
            command = await _inputOutput.ReadCommandAsync(cancellationToken);
        }
    }


    private void ProcessStartupCommand(UciCommand command) {
        if (command is { Command: "id" }) {
            ProcessIdCommand(command);
        } else if (command is { Command: "option" }) {
            ProcessOptionCommand(command);
        }
    }

    private void ProcessIdCommand(UciCommand command) {
        if (command.Parameters is ["name", ..]) {
            Name = string.Join(" ", command.Parameters.Skip(1));
        } else if (command.Parameters is ["author", ..]) {
            Authors = string.Join(" ", command.Parameters.Skip(1));
        }
    }

    private void ProcessOptionCommand(UciCommand command) {
        
    }
}