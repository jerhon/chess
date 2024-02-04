namespace Honlsoft.Chess.Serialization.Pgn;

/// <summary>
/// Represents a PGN move.
/// </summary>
public record PgnMove(int MoveNumber, PieceColor Color, San Move, string? Comment);