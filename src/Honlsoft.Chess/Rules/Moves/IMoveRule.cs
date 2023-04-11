namespace Honlsoft.Chess.Rules; 

public interface IMoveRule {

    /// <summary>
    /// Determines if a rule is applicable for a particular piece on a square.
    /// </summary>
    /// <param name="chessBoard"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    bool IsApplicable(IChessBoard chessBoard, SquareName from);

    /// <summary>
    /// Get the possible moves for a chess piece on a square of the chess board.
    /// </summary>
    /// <param name="chessBoard">The chess board to evaluate.</param>
    /// <param name="from">The position to move from.</param>
    /// <returns>The array of squares for possible moves.</returns>
    SquareName[] GetPossibleMoves(IChessBoard chessBoard, SquareName from);
    
}