using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Engine; 



/// <summary>
/// Provides a chess engine.  It's assummed the engine is tied to an individual chess game and as such has access to the current game's state.
/// </summary>
public interface IChessEngine {
    
    /// <summary>
    /// Starts a game, gives the engine a chance to initialize if necessary.
    /// </summary>
    public Task StartGameAsync(CancellationToken cancellationToken);


    // TODO: should have something better for movestring

    public void AddMove(string moveString);
    
    /// <summary>
    /// Sends a move to the engine.
    /// </summary>
    /// <returns></returns>
    public Task UpdatePositionAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Suggests a move from the engine.
    /// </summary>
    /// <returns></returns>
    public Task<EngineSuggestion> SuggestMoveAsync(CancellationToken cancellationToken);
    
    
}