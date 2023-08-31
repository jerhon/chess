namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciCommandBuilder {


    private readonly List<UciParameter> _parameters = new();
    private string _command;


    public UciCommandBuilder WithCommand(string command) {
        _command = command;
        return this;
    }

    public UciCommandBuilder WithParameter(string key, string value) {
        _parameters.Add(new UciParameter{ Key = key, Value = value });
        return this;
    }

    public UciCommand Build() {
        if (_command == null) {
            throw new InvalidOperationException("No command set.");
        }

        return new UciCommand(_command, _parameters);
    }
}