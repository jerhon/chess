namespace Honlsoft.Chess.Rules;

public record ChessMove(SquareName From, SquareName To) {

    /// <summary>
    /// The EnPassant capture, if this is an en-passant move, the position of the piece to capture.
    /// </summary>
    public SquareName? EnPassantCapture { get; set; }
    
    /// <summary>
    /// The EnPassant target, if this is a pawn move this would be the move a chess piece could move to to capture this piece en-passant.
    /// </summary>
    public SquareName? EnPassantTarget { get; set; }

    /// <summary>
    /// Returns whether the move requires promotion.
    /// </summary>
    public bool RequiresPromotion { get; set; } = false;

}