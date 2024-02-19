namespace Honlsoft.Chess;


/// <summary>
/// 
/// </summary>
public record MoveDifference {
    
    /// <summary>
    /// Squares where pieces have been added.
    /// </summary>
    public IEnumerable<Square> Additions { get; set; }
    
    /// <summary>
    /// Squares where pieces have been removed.
    /// </summary>
    public IEnumerable<Square> Removals { get; set; }
    
    /// <summary>
    /// The new en-passant square.
    /// </summary>
    public SquareName? EnPassant { get; set; }
}