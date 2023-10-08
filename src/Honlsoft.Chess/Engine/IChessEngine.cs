using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Engine; 



/// <summary>
/// Provides a chess engine.  It's assummed the engine is tied to an individual chess game and as such has access to the current game's state.
/// </summary>
public interface IChessEngine {
    
    /// <summary>
    /// Starts a game with the position set out by the chess board.
    /// </summary>
    /// <param name="chessBoard">The chess board.</param>
    /// <param name="moves">The moves.</param>
    /// <returns></returns>
    public Task StartGameAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Suggests a move from the engine.
    /// </summary>
    /// <returns></returns>
    public Task<EngineSuggestion> SuggestMoveAsync(CancellationToken cancellationToken);
    
    
}