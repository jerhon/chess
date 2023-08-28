namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciCommandBuilder {


    private List<string>? _parameters = new List<string>();
    private string _command;


    public UciCommandBuilder WithCommand(string command) {
        _command = command;
        return this;
    }

    public UciCommandBuilder WithParameter(string parameter) {
        _parameters.Add(parameter);
        return this;
    }

    public UciCommand Build() {
        if (_command == null) {
            throw new InvalidOperationException("No command set.");
        }

        return new UciCommand(_command, _parameters);
    }
}