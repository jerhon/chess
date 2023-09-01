namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciCommand {

    public UciCommand(string command, IEnumerable<UciParameter?>? parameters = null) {
        Command = command;
        if (parameters == null) {
            Parameters = new List<UciParameter>();
        } else {
            Parameters = new List<UciParameter>(parameters.Where((p) => p is not null)!);   
        }
    }

    /// <summary>
    /// The command.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// The parameters for the command.
    /// </summary>
    public IReadOnlyList<UciParameter> Parameters { get; }
}