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


    /// <summary>
    /// Retrieves a parameter by name.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <returns>The value, or null if it does not exist.</returns>
    public string? GetParameter(string key) {
        return Parameters.FirstOrDefault((p) => p.Key == key)?.Value;
    }
}