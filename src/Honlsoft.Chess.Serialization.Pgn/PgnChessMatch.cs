namespace Honlsoft.Chess.Serialization; 

public class PgnChessMatch(IEnumerable<PgnTag> attributes) {

    public List<PgnTag> Tags { get; } = new(attributes);

    public List<PgnMovePart> Moves { get; } = new();

    public string Result { get; set; }
}