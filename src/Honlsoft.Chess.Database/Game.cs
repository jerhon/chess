namespace Honlsoft.Chess.Database;

public class Game
{
    /// <summary>
    /// The ID of the game.
    /// </summary>
    public Guid GameId { get; set; }
    
    /// <summary>
    /// The Full PGN of the game.
    /// </summary>
    public string Pgn { get; set; }
   
    /// <summary>
    /// The positions in the game.
    /// </summary>
    public ICollection<GamePosition> Positions { get; set; } = new List<GamePosition>();
}