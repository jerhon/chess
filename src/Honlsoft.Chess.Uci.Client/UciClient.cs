using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Channels;
using Honlsoft.Chess.Serialization;
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

    public async Task IsReadyAsync(CancellationToken cancellationToken) {

        await _inputOutput.SendCommandAsync(new UciCommand("isready"), cancellationToken);

        await WaitForCommandAsync("readyok", cancellationToken);
    }

    public async Task SetOptionAsync(string name, string? value, CancellationToken cancellationToken) {

        UciCommandBuilder builder = new();
        builder.WithCommand("setoption");
        builder.WithParameter("name", name);
        if (value is not null) {
            builder.WithParameter("value", value);
        }
        UciCommand command = builder.Build();

        await _inputOutput.SendCommandAsync(command, cancellationToken);
        
        // There is no response for this
    }

    public async Task SetFenPositionAsync(string fenString, string[] moves, CancellationToken cancellationToken) {
        UciCommandBuilder builder = new();
        builder.WithCommand("position");
        builder.WithParameter("fen", fenString);
        builder.WithParameter("moves", string.Join(" ", moves));
        UciCommand command = builder.Build();

        await _inputOutput.SendCommandAsync(command, cancellationToken);
    }

    public async Task SetStartingPositionAsync(San[] moves, CancellationToken cancellationToken) {
        UciCommandBuilder builder = new();
        builder.WithCommand("position");
        builder.WithParameter("startpos", null);
        builder.WithParameter("moves", string.Join(" ", moves.Select((m) => m.ToString())));
        UciCommand command = builder.Build();

        await _inputOutput.SendCommandAsync(command, cancellationToken);
    }

    public async Task SetMovePositionAsync(string[] moves, CancellationToken cancellationToken) {
        UciCommandBuilder builder = new();
        builder.WithCommand("position");
        builder.WithParameter("moves", string.Join(" ", moves));
        UciCommand command = builder.Build();

        await _inputOutput.SendCommandAsync(command, cancellationToken);
    }

    public async Task UciNewGameAsync(CancellationToken cancellationToken) {
        await _inputOutput.SendCommandAsync(new UciCommand("ucinewgame"), cancellationToken);
    }

    /// <summary>
    /// Sends a UCI go command returning a channel with the data UCI commands it responds with.
    /// </summary>
    /// <param name="parameters">The parameters for the go command.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the go command.</param>
    /// <returns></returns>
    public async Task<Channel<UciCommand>> GoAsync(GoParameters parameters, CancellationToken cancellationToken) {
        UciCommandBuilder commandBuilder = new UciCommandBuilder();
        commandBuilder.WithCommand("go");
        if (parameters.MoveTime.HasValue) {
            commandBuilder.WithParameter("movetime", Math.Round(parameters.MoveTime.Value.TotalMilliseconds, 0).ToString(CultureInfo.CurrentCulture));
        }
        if (parameters.Infinite ?? false) {
            commandBuilder.WithParameter("infinite", null);
        }

        await _inputOutput.SendCommandAsync(commandBuilder.Build(), cancellationToken);

        UciCommand? command = null;
        Channel<UciCommand> commandChannel = Channel.CreateUnbounded<UciCommand>();


        // This runs in the background as the goal is to have it wait for the bestmove command to go through.
 #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(async () => {
 #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            try {
                while (command?.Command != "bestmove") {

                    command = await _inputOutput.ReadCommandAsync(cancellationToken);
                    if (command != null) {
                        await commandChannel.Writer.WriteAsync(command, cancellationToken);
                    }
                }
            }
            catch (Exception ex) {
                commandChannel.Writer.Complete(ex);
            }
            finally {
                commandChannel.Writer.Complete();
            }
        });

        return commandChannel;
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        await _inputOutput.SendCommandAsync(new UciCommand("stop"), cancellationToken);
    }
    
    public async Task<UciCommand?> WaitForCommandAsync(string commandName, CancellationToken cancellationToken) {
        UciCommand? command = null;
        while (command?.Command != commandName) {
            command = await _inputOutput.ReadCommandAsync(cancellationToken);
        }
        return command;
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