namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciCommandSerializer {
    
    
    /// <summary>
    /// Commands are serialized by 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public string SerializeCommand(UciCommand command) {
        var parameters = string.Join(" ", command.Parameters);
        return $"{command.Command} {parameters}\n";
    }

    public UciCommand DeserializeCommand(string rawCommand) {
        // This is a pretty naive implementation, need to double check if there is anything with strings, etc.
        // there are certain commands that have values that include whitespace, like name.  However, I'm not going to deal with those for now.
        var parts = rawCommand.Split(new[] { '\n', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) {
            return null;
        }
        var command = parts[0];

        // If this is a simple command with no additional parameters.
        if (parts.Length == 1) {
            return new UciCommand(parts[0]);
        }
        
        // Now for the more complicated ones with specific parameters
        if (command is "option") {
            return ParseComplexCommand("name", "type", "default", "min", "max", "var");
        }
        else if (command is "id") {
            return ParseComplexCommand("name");
        }
        else if (command is "info") {
            // TODO: score has sub values, not sure what to do with those for now
            return ParseComplexCommand("depth", "seldepth", "time", "nodes", "pv", "multipv", "score", "cp", "mate", "lowerbound", "upperbound",
                "currmove", "currmovenumber", "hashfull", "nps", "tbhits", "sbhits", "cpuload", "refutation", "currline");
        } else if (command is "setoption") {
            return ParseComplexCommand("name", "value");
        } else if (command is "position") {
            return ParseComplexCommand("fen", "startpos", "moves");
        } else if (command is "go") {
            return ParseComplexCommand("searchmoves", "ponder", "wtime", "btime", "winc", "binc", "movestogo", "depth", "nodes", "mate", "movetime",
                "infinite");
        }

        // if all else fails, just create the command by using the first part as the command, and split the rest for the parameters
        return new UciCommand(parts[0], parts.Skip(1));
    }

    private bool EqualsAt(string source, int index, string value) {
        if (source.Length >= index + value.Length) {
            return source.Substring(index, value.Length) == value;
        }
        return false;
    }

    public UciCommand ParseComplexCommand(string rawCommand, params string[] parameterKeys) {
        List<string> parameters = new List<string>();
        int indexOf = rawCommand.IndexOf(' ');
        int lastKeyStart = rawCommand.IndexOf(' ');

        var command = rawCommand.Substring(0, indexOf);
        while (indexOf >= 0) {
            var key = parameterKeys.FirstOrDefault((k) => EqualsAt(rawCommand, indexOf, k));
            if (key != null) {
                if (indexOf != lastKeyStart) {
                    var keyValue = rawCommand.Substring(lastKeyStart, indexOf - lastKeyStart);
                    parameters.Add(keyValue);
                }
                lastKeyStart = indexOf;
                indexOf = rawCommand.IndexOf(' ', indexOf + 1);
            }
        }

        // Add the final key parameter to the values.
        if (lastKeyStart > 0) {
            parameters.Add(rawCommand.Substring(lastKeyStart));
        }

        return new UciCommand(command, parameters);
    }
}