namespace Honlsoft.Chess; 

public interface IChessGame {
    
    IChessPosition CurrentPosition { get; }
    
    /// <summary>
    /// The current game state, Player to Move, Check, Checkmate, etc.
    /// </summary>
    ChessGameState GameState { get; }

    /// <summary>
    /// Makes a move.
    /// </summary>
    /// <param name="from">The from square.</param>
    /// <param name="to">The to square.</param>
    /// <param name="promotionPiece">The promotion piece.</param>
    MoveResult Move(SquareName from, SquareName to, PieceType? promotionPiece);

    /// <summary>
    /// Castles a king.
    /// </summary>
    /// <param name="side">The side to castle to.</param>
    MoveResult Castle(CastlingSide side);

}