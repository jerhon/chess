namespace Honlsoft.Chess.Uci.Client; 

public class GoParameters {

    public string[] ChessMoves { get; set; }
    
    public bool? Ponder { get; set; }
    
    public TimeSpan? MoveTime { get; set; }
    
    public bool? Infinite { get; set; }
    
}