using Honlsoft.Chess.Serialization.Pgn;

namespace Honlsoft.Chess.Serialization;

public record PgnChessMatch(PgnTag[] Tags, PgnMove[] Moves);