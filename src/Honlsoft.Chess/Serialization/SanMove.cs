namespace Honlsoft.Chess.Serialization; 

public record SanMove : San
{

    private static SanSerializer _sanSerializer = new SanSerializer();

    public PieceType? FromPiece { get; init; }
    
    public SquareFile? FromFile { get; init; }
    
    public SquareRank? FromRank { get; init; }

    public required SquareFile ToFile { get; init; }
    
    public required SquareRank ToRank { get; init; }
    
    public bool Capture { get; init; } = false;
    
    public PieceType? PromotionPiece { get; init; }

    public SanCheckType? Check { get; init; }

    public override string ToString()
    {
        return _sanSerializer.Serialize(this);
    }


    public static SanMove From(SquareName from, SquareName to)
    {
        SanMove move = new SanMove
        {
            FromFile = from.SquareFile,
            FromRank = from.SquareRank,
            ToFile = to.SquareFile,
            ToRank = to.SquareRank
        };
        return move;
    }
}