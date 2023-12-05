namespace Honlsoft.Chess.Rules; 


/// <summary>
/// Contains logic for moving a piece on a chess board.
/// </summary>
public interface IChessMove {

    /// <summary>
    /// The square the move is from.
    /// </summary>
    SquareName From { get; }
    
    /// <summary>
    /// Checks if this candidate move is a match for a move from one square to another.
    /// </summary>
    /// <param name="from">The from square.</param>
    /// <param name="to">The to square.</param>
    /// <returns></returns>
    SquareName To { get; }

    /// <summary>
    /// Applies a move to a chess board.
    /// </summary>
    /// <param name="chessGame">The chess game to apply the move to.</param>
    void Move(IChessGame chessGame);
}