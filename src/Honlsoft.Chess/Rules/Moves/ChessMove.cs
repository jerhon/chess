namespace Honlsoft.Chess.Rules;

public record ChessMove(SquareName From, SquareName To) {

    public SquareName? EnPassantCapture { get; set; }
        
    
    public SquareName? EnPassantTarget { get; set; }
    
}