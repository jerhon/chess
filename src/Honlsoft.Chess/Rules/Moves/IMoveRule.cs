namespace Honlsoft.Chess.Rules; 

public interface IMoveRule {

    /// <summary>
    /// Determines if a rule is applicable for a particular piece on a square.
    /// </summary>
    /// <param name="chessPosition"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    bool IsApplicable(IChessPosition chessPosition, SquareName from);

    /// <summary>
    /// Get the possible moves for a chess piece on a square of the chess board.  This does not take threats into account, just the possible squares a piece can move to.
    /// </summary>
    /// <param name="chessPosition">The chess board to evaluate.</param>
    /// <param name="from">The position to move from.</param>
    /// <returns>The array of squares for possible moves.</returns>
    IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from);


}