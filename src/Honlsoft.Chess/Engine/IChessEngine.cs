using System.Threading.Channels;
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
    /// Starts calculating, returning lines the engine has calculated.
    /// </summary>
    /// <returns></returns>
    public Task<Channel<EngineLine>> StartCalculatingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stop calculating, returning the best move that was calculated.
    /// </summary>
    /// <param name="cancelToken">Cancel the stop.</param>
    /// <returns>A task with the best move.</returns>
    public Task<BestMove> StopCalculatingAsync(CancellationToken cancelToken);


}