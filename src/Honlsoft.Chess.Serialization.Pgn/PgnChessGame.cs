namespace Honlsoft.Chess.Serialization; 

public class PgnChessGame(IEnumerable<PgnAttribute> attributes) {

    public List<PgnAttribute> Attributes { get; } = new(attributes);

    public List<PgnMove> Moves { get; } = new();

}