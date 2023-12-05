namespace Honlsoft.Chess.Serialization; 

public record SanMove : San {

    public PieceType? FromPiece { get; init; }
    
    public SquareFile? FromFile { get; init; }
    
    public SquareRank? FromRank { get; init; }

    public required SquareFile ToFile { get; init; }
    
    public required SquareRank ToRank { get; init; }
    
    public bool Capture { get; init; } = false;
    
    public PieceType? PromotionPiece { get; init; }

    public SanCheckType? Check { get; init; }

    
}