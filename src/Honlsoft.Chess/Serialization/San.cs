using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess; 

public record San {

    public PieceType? FromPiece { get; init; }
    
    public SquareFile? FromFile { get; init; }
    
    public SquareRank? FromRank { get; init; }

    public SquareFile? ToFile { get; init; }
    
    public SquareRank? ToRank { get; init; }
    
    public bool Capture { get; init; } = false;
    
    public PieceType? PromotionPiece { get; init; }

    public SanCheckType? Check { get; init; }

    
}