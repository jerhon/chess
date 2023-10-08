namespace Honlsoft.Chess; 

public interface IMoveCounter {

    /// <summary>
    /// Gets the total number of pieces that can move to this square.
    /// </summary>
    /// <param name="square">The name of the square</param>
    /// <returns>The total number of times the square can be moved to.</returns>
    int GetMoveCount(SquareName square);
}