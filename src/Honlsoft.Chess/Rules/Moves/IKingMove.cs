namespace Honlsoft.Chess.Rules; 

/// <summary>
/// An interface for a king move
/// </summary>
public interface IKingMove {
    
    public SquareName[] GetThreateningSquares();
}