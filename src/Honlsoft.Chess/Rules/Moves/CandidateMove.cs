namespace Honlsoft.Chess.Rules; 

public record CandidateMove(SquareName Square, bool EnPassant = false);