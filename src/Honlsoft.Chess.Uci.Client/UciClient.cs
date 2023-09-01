using System.Collections.ObjectModel;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

public class UciClient {
    private readonly IUciInputOutput _inputOutput;
    private readonly Dictionary<string, UciOption> _options = new Dictionary<string, UciOption>();
    
    public UciClient(IUciInputOutput inputOutput) {
        _inputOutput = inputOutput;
        Options = new ReadOnlyDictionary<string, UciOption>(_options);
    }
    
    public string Name { get; private set; }
    public string Author { get; private set; }
    
    public ReadOnlyDictionary<string, UciOption> Options { get; }

    /// <summary>
    /// Sends the UCI command to initialize the interface.
    /// </summary>
    public async Task UciAsync(CancellationToken cancellationToken) {
        // Start it with the UCI command
        await _inputOutput.SendCommandAsync(new UciCommand("uci"), cancellationToken);
        
        // Go through and process each response
        UciCommand? command = null;
        while (command is not { Command: "uciok" }) {
            command = await _inputOutput.ReadCommandAsync(cancellationToken);
            if (command != null) {
                ProcessStartupCommand(command);
            }
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
        var firstParameter = command.Parameters.FirstOrDefault();
        if (firstParameter != null) {
            if (firstParameter.Key == "author") {
                Author = firstParameter.Value;
            } else if (firstParameter.Key == "name") {
                Name = firstParameter.Value;
            }
        }
    }

    private void ProcessOptionCommand(UciCommand command) {
        UciOption option = new UciOption(command);
        _options.Add(option.Name, option);
    }
}