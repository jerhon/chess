namespace Honlsoft.Chess.Serialization.Pgn; 

public class PgnWriter(TextWriter writer) {

    public void WriteTag(PgnTag tag) {
        var stringValue = EscapeString(tag.Value);
        writer.Write($"[{tag.Name} {stringValue}]\n");
    }

    public void WritePgnChessMatch(PgnChessMatch chessMatch) {
        foreach (var tag in chessMatch.Tags) {
            
        }
    }

    public static string EscapeString(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
}