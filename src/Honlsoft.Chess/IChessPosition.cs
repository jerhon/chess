namespace Honlsoft.Chess; 

public interface IChessPosition {
     
     /// <summary>
     /// Retrieves a square from the chess baard.
     /// </summary>
     /// <param name="squareName">The name of the square.</param>
     /// <returns>The square representation</returns>
     Square GetSquare(SquareName squareName);
     
     /// <summary>
     /// The square a pawn has moved over in the last move. 
     /// </summary>
     SquareName? EnPassantTarget { get; }
     
     /// <summary>
     /// The current player to move.
     /// </summary>
     PieceColor PlayerToMove { get; }
     
     /// <summary>
     /// Moves in the games.
     /// </summary>
     int FullMoves { get; }

     /// <summary>
     /// Half moves (player moves since the last pawn or piece capture).
     /// </summary>
     int HalfMoves { get; }
     
     /// <summary>
     /// Returns the castling rights for a player.
     /// </summary>
     /// <param name="playerColor">The player's piece color.</param>
     /// <returns>The castling rights for the player.</returns>
     CastlingSide[] GetCastlingRights(PieceColor playerColor);

     /// <summary>
     /// Returns whether a player still has castling rights.
     /// </summary>
     /// <param name="playerColor">The player color.</param>
     /// <param name="castleSide">The castling side.</param>
     /// <returns></returns>
     bool CanCastle(PieceColor playerColor, CastlingSide castleSide);
}