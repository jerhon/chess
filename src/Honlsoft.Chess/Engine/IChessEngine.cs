using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess.Engine; 

public interface IChessEngine {
    
    /// <summary>
    /// Starts a game with the position set out by the chess board.
    /// </summary>
    /// <param name="chessBoard">The chess board.</param>
    /// <param name="moves">The moves.</param>
    /// <returns></returns>
    public Task StartGameAsync(IChessBoard chessBoard, ChessMove[] moves);
    
    /// <summary>
    /// Sends a new move 
    /// </summary>
    /// <param name="move">The move for the chess engine.</param>
    /// <returns>A task that completes when the move has been sent.</returns>
    public Task SendMoveAsync(ChessMove move);
    
    /// <summary>
    /// Suggests a move from the engine.
    /// </summary>
    /// <returns></returns>
    public Task<string> SuggestMoveAsync();
}