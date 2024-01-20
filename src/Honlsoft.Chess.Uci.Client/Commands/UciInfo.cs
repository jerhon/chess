namespace Honlsoft.Chess.Uci.Client.Commands;

public class UciInfo {


    public UciInfo(UciCommand command) {
        foreach (var parameter in command.Parameters) {
            if (parameter.Key == "depth") {
                if (int.TryParse(parameter.Value, out int result)) {
                    Depth = result;
                }
            } else if (parameter.Key == "seldepth") {
                if (int.TryParse(parameter.Value, out int result)) {
                    SelDepth = result;
                }
            } else if (parameter.Key == "time") {
                if (int.TryParse(parameter.Value, out int result)) {
                    Time = result;
                }
            } else if (parameter.Key == "nodes") {
                if (int.TryParse(parameter.Value, out int result)) {
                    Nodes = result;
                }
            } else if (parameter.Key == "cp") {
                if (int.TryParse(parameter.Value, out int result)) {
                    CentiPawns = result;
                }
            } else if (parameter.Key == "mate") {
                if (int.TryParse(parameter.Value, out int result)) {
                    Mate = result;
                }
            } else if (parameter.Key == "pv") {
                pv = parameter.Value;
            } else if (parameter.Key == "currmove") {
                CurrentMove = parameter.Value;
            } else if (parameter.Key == "currmovenumber") {
                CurrentMoveNumber = parameter.Value;
            } else if (parameter.Key == "hashfull") {
                HashFull = parameter.Value;
            } else if (parameter.Key == "nps") {
                Nps = parameter.Value;
            } else if (parameter.Key == "tbhits") {
                TbHits = parameter.Value;
            } else if (parameter.Key == "cpuload") {
                Cpuload = parameter.Value;
            } else if (parameter.Key == "string") {
                String = parameter.Value;
            } else if (parameter.Key == "refutation") {
                Refutation = parameter.Value;
            } else if (parameter.Key == "currline") {
                CurLine = parameter.Value;
            }
        }
    }

    public int? Depth { get; }

    public int? SelDepth { get; }

    public int? Time { get; }

    public int? Nodes { get; }

    public string? pv { get; }

    public int? CentiPawns { get; }

    public int? Mate { get; }

    public string? CurrentMove { get; }

    public string? CurrentMoveNumber { get; }

    public string? HashFull { get; }

    public string? Nps { get; }

    public string? TbHits { get; }

    public string? Cpuload { get; }

    public string? String { get; }

    public string? Refutation { get; }

    public string? CurLine { get; }

}