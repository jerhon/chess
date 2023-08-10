namespace Honlsoft.Chess.GameService.Model; 

public interface IChessGameStorage {

    /// <summary>
    /// Returns a chess game by it's ID.
    /// </summary>
    /// <param name="gameId">The ID of the game.</param>
    /// <returns>The chess game.</returns>
    public ChessGame? GetGameState(Guid gameId);


    public Guid NewGame();
}