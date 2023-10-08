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
     /// Returns whether a player still has castling rights.
     /// </summary>
     /// <param name="playerColor">The player color.</param>
     /// <param name="castleSide">The castling side.</param>
     /// <returns></returns>
     bool CanCastle(PieceColor playerColor, CastlingSide castleSide);
}