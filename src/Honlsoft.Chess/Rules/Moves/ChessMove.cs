namespace Honlsoft.Chess.Rules; 

public record ChessMove(SquareName FromSquare, SquareName ToSquare, SquareName? EnPassantCapture = null);