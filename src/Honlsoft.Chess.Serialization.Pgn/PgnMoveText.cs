using Honlsoft.Chess;

namespace Honlsoft.Chess.Serialization.Pgn;

/// <summary>
/// Text associated with a move.
/// </summary>
public record PgnMoveText(San SanMove) : PgnMovePart {
    
    
    public static PgnMoveText Parse(string moveText) {
        SanSerializer serializer = new SanSerializer();
        var san = serializer.Deserialize(moveText);
        return new PgnMoveText(san);
    }
}