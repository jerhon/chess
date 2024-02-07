namespace Honlsoft.Chess.Serialization;

public record SanCastle : San {
    
    public required CastlingSide Side { get; init; }

    public SanCheckType? Check { get; init; } = null;

}