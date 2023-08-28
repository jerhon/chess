using System.Text;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

// TODO: will want to make this multi-thread safe eventually


public class StreamUciInputOutput : IUciInputOutput {
    private readonly StreamReader _readStream;
    private readonly StreamWriter _writeStream;
    private readonly UciCommandSerializer _serializer;

    public StreamUciInputOutput(UciCommandSerializer serializer, StreamReader readStream, StreamWriter writeStream) {
        _readStream = readStream;
        _writeStream = writeStream;
        _serializer = serializer;
    }

    public async Task SendCommandAsync(UciCommand command, CancellationToken cancellationToken) {
        var commandSerialized = _serializer.SerializeCommand(command);
        await _writeStream.WriteAsync(commandSerialized.ToCharArray(), cancellationToken);
    }
    
    public async Task<UciCommand?> ReadCommandAsync(CancellationToken cancellationToken) {
        var rawCommand = await _readStream.ReadLineAsync(cancellationToken);
        if (string.IsNullOrEmpty(rawCommand)) {
            return null;
        }
        var command = _serializer.DeserializeCommand(rawCommand);
        return command;
    }
}