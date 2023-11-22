using System.Text;
using Honlsoft.Chess.Uci.Client.Commands;
using Microsoft.Extensions.Logging;

namespace Honlsoft.Chess.Uci.Client; 

// TODO: will want to make this multi-thread safe eventually


public class StreamUciInputOutput(
    UciCommandSerializer serializer,
    StreamReader readStream,
    StreamWriter writeStream,
    ILogger<StreamUciInputOutput> logger)
    : IUciInputOutput {


    public async Task SendCommandAsync(UciCommand command, CancellationToken cancellationToken) {
        var commandSerialized = serializer.SerializeCommand(command);
        
        logger.LogTrace("Sending UCI Command: {CommandText}", commandSerialized);
        
        await writeStream.WriteAsync(commandSerialized.ToCharArray(), cancellationToken);
    }
    
    public async Task<UciCommand?> ReadCommandAsync(CancellationToken cancellationToken) {
        var rawCommand = await readStream.ReadLineAsync(cancellationToken);
        
        logger.LogTrace("Received UCI Command: {CommandText}", rawCommand);
        
        if (string.IsNullOrEmpty(rawCommand)) {
            return null;
        }
        
        var command = serializer.DeserializeCommand(rawCommand);
        return command;
    }
}