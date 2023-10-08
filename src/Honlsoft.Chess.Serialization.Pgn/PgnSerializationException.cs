namespace Honlsoft.Chess.Serialization.Pgn; 

public class PgnSerializationException : Exception {

    public PgnSerializationException(string message, int line, int offset) : base(message) {
        Line = line;
        Offset = offset;
    }
    
    public int Line { get; }
    
    public int Offset { get; }
}