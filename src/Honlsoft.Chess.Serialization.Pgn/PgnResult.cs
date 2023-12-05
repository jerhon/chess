using System.Xml;

namespace Honlsoft.Chess.Serialization.Pgn;

public record PgnResult(decimal White, decimal Black) : PgnMovePart {
    
}