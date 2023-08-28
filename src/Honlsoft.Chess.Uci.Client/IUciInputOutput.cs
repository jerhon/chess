using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client;


public interface IUciInputOutput {
    
    /// <summary>
    /// Sends a command to the UCI interface.
    /// </summary>
    /// <param name="command">The command to send.</param>
    Task SendCommandAsync(UciCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Reads a command from the UCI interface.
    /// </summary>
    /// <returns>The command that was read.</returns>
    Task<UciCommand?> ReadCommandAsync(CancellationToken cancellationToken);
    
}