using System.Reflection;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client.Tests; 

public class FakeUciInterface : IUciInputOutput {

    private readonly List<Action<UciCommand, FakeUciInterface>> _responders = new();
    private readonly Queue<UciCommand?> _responses = new();

    public List<UciCommand> Requests { get; } = new();

    public bool HasResponses => _responses.Count > 0;
    
    public void AddResponses(params UciCommand?[] responses) {
        foreach (var response in responses) {
            _responses.Enqueue(response);
        }
    }

    public void AddResponder(Action<UciCommand, FakeUciInterface> responder) {
        _responders.Add(responder);
    }


    public void ReadResponses(string fileName) {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
        string actualPath = Path.Combine(fileName);

        var lines = File.ReadAllLines(actualPath);
        var uciSerializer = new UciCommandSerializer();

        var commands = lines.Select((l) => uciSerializer.DeserializeCommand(l));
        foreach (var command in commands) {
            _responses.Enqueue(command);
        }
    }
    
    
    public Task SendCommandAsync(UciCommand command, CancellationToken cancellationToken) {

        Requests.Add(command);
        
        foreach (var responder in _responders) {
            responder(command, this);
        }

        return Task.CompletedTask;
    }
    
    public Task<UciCommand?> ReadCommandAsync(CancellationToken cancellationToken) {
        if (_responses.Count > 0) {
            return Task.FromResult(_responses.Dequeue())!;
        }

        return Task.FromResult<UciCommand?>(null);
    }
}