namespace Honlsoft.Chess.Rules;

public record ChessMove(SquareName FromSquare, SquareName ToSquare) {

    public SquareName? EnPassantCapture { get; set; }
        
    
    public SquareName? EnPassantTarget { get; set; }
    
}