namespace Honlsoft.Chess.Serialization.Pgn; 

/// <summary>
/// Text associated with a move.
/// </summary>
public record PgnMoveText(string Move) : PgnMovePart;