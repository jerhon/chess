namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciCommand {

    public UciCommand(string command, IEnumerable<string>? parameters = null) {
        Command = command;
        if (parameters == null) {
            Parameters = new List<string>();
        } else {
            Parameters = new List<string>(parameters);   
        }
    }

    /// <summary>
    /// The command.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// The parameters for the command.
    /// </summary>
    public IReadOnlyList<string> Parameters { get; }
}