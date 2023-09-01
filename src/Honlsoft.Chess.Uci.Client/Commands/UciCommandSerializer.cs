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
            return ParseComplexCommand(rawCommand, "name", "type", "default", "min", "max", "var");
        } else if (command is "id") {
            return ParseComplexCommand(rawCommand, "name", "author");
        } else if (command is "register") {
            return ParseComplexCommand(rawCommand, "later", "name", "code");
        } else if (command is "info") {
            // TODO: score has sub values, not sure what to do with those for now
            return ParseComplexCommand(rawCommand, "depth", "seldepth", "time", "nodes", "pv", "multipv", "score", "cp", "mate", "lowerbound",
                "upperbound",
                "currmove", "currmovenumber", "hashfull", "nps", "tbhits", "sbhits", "cpuload", "refutation", "currline");
        } else if (command is "setoption") {
            return ParseComplexCommand(rawCommand, "name", "value");
        } else if (command is "position") {
            return ParseComplexCommand(rawCommand, "fen", "startpos", "moves");
        } else if (command is "go") {
            return ParseComplexCommand(rawCommand, "searchmoves", "ponder", "wtime", "btime", "winc", "binc", "movestogo", "depth", "nodes", "mate",
                "movetime",
                "infinite");
        }

        var remainder = rawCommand.Length > command.Length + 1
            ? new[] { new UciParameter(null, rawCommand.Substring(command.Length + 1)) }
            : null;

        // if all else fails, just create the command by using the first part as the command, and split the rest for the parameters
        return new UciCommand(parts[0], remainder);
    }

    private bool EqualsAt(string source, int index, string value) {
        if (index < 0) {
            return false;
        }
        if (source.Length >= index + value.Length) {
            return source.Substring(index, value.Length) == value;
        }
        return false;
    }

    private UciCommand ParseComplexCommand(string rawCommand, params string[] parameterKeys) {
        int nextSpace = rawCommand.IndexOf(' ');
        

        if (nextSpace < 0) {
            return new UciCommand(rawCommand);
        }
        
        // the command is the first part...
        var command = rawCommand.Substring(0, nextSpace);
        var parameters = GetUciParameters(rawCommand, nextSpace, parameterKeys);
        
        return new UciCommand(command, parameters);
    }

    private UciParameter[] GetUciParameters(string rawCommand, int startingIdx, string[] parameterKeys) {
        var rawParameters = ParseRawParameters(rawCommand, startingIdx, parameterKeys);
        var uciParameters = rawParameters.Select((p) => GetUciParameter(p, parameterKeys)).ToArray();
        return uciParameters;
    }


    private UciParameter GetUciParameter(string rawParameter, string[] parameterKeys) {

        if (parameterKeys.Any((k) => rawParameter.StartsWith(k))) {

            int spaceIdx = rawParameter.IndexOf(' ');
            if (spaceIdx > 0) {
                var name = rawParameter.Substring(0, spaceIdx);
                var value = rawParameter.Substring(spaceIdx + 1);
                return new UciParameter(name, value);
            } else {
                return new UciParameter(rawParameter, "");
            }

        }

        return new UciParameter(null, rawParameter);
    }
    
    
    public List<string> ParseRawParameters(string rawCommand, int startingIdx, string[] parameterKeys) {
        var keywordPositions = GetKeyPositions(rawCommand, startingIdx, parameterKeys);
        List<string> parameters = new List<string>();

        for (int i = 0; i < keywordPositions.Count - 1; i++) {
            var currentPosition = keywordPositions[i];
            var nextPosition = keywordPositions[i + 1];
            
            parameters.Add(rawCommand.Substring(currentPosition, nextPosition - currentPosition).Trim());
        }

        if (keywordPositions.Count > 0 && rawCommand.Length > keywordPositions[^1]) {
            parameters.Add(rawCommand.Substring(keywordPositions[^1]));
        }

        return parameters;
    }
    
    

    private List<int> GetKeyPositions(string rawCommand, int startIdx, string[] parameterKeys) {
        List<int> keywordPositions = new List<int>();
        int idx = startIdx;
        while (idx >= 0) {
            if (parameterKeys.Any((k) => EqualsAt(rawCommand, idx + 1, k))) {
                keywordPositions.Add(idx + 1);
            }

            idx = rawCommand.IndexOf(' ', idx + 1);
        }
        return keywordPositions;
    }
}