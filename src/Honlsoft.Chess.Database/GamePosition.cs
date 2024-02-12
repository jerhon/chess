using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Honlsoft.Chess.Database;

public class GamePosition
{
    public Guid GamePositionId { get; set; }
    
    /// <summary>
    /// The ID of the chess position.
    /// </summary>
    public Guid GameId { get; set; }
    
    /// <summary>
    /// The Move number in the game.  Index starts at 1.
    /// </summary>
    public int? MoveNumber { get; set; }

    /// <summary>
    /// The player to move.  "w" for white, "b" for black.
    /// </summary>
    public PieceColor? PlayerToMove  { get; set; }
    
    /// <summary>
    /// The previous position hash.  Used to identify similar positions.
    /// </summary>
    public ulong? PreviousHash { get; set; }
    
    /// <summary>
    /// Hash of the position.  Used to identify similar positions.
    /// </summary>
    public ulong? Hash { get; set; }
   
    /// <summary>
    /// The current piece positions for the game.
    /// </summary>
    public ChessPosition? Position { get; set; }
    
    /// <summary>
    /// Half moves.
    /// </summary>
    public int HalfMoves { get; set; }
    
    /// <summary>
    /// Full moves.
    /// </summary>
    public int FullMoves { get; set; }
    
    public Game? Game { get; set; }

}