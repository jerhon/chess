namespace Honlsoft.Chess.Database;

public class ChessPosition {
    
    /// <summary>
    /// The ID of the position.
    /// </summary>
    public Guid ChessPositionId { get; set; }
    
    /// <summary>
    /// The Hash of the position.
    /// </summary>
    public ulong Hash { get; set; }
   
    /// <summary>
    /// The FEN string for the position.
    /// </summary>
    public string Fen { get; set; }
    
    /// <summary>
    /// The En Passant target square.
    /// </summary>
    public string? EnPassantTarget { get; set; }
    
    /// <summary>
    /// Returns true if white can castle king side.
    /// </summary>
    public bool? WhiteCanCastleKingSide { get; set; }
    
    /// <summary>
    /// Returns true if white can castle queen side.
    /// </summary>
    public bool? WhiteCanCastleQueenSide { get; set; }
    
    /// <summary>
    /// Returns true if black can castle king side.
    /// </summary>
    public bool? BlackCanCastleKingSide { get; set; }
    
    /// <summary>
    /// Returns true if black can castle queen side.
    /// </summary>
    public bool? BlackCanCastleQueenSide { get; set; }
    
}