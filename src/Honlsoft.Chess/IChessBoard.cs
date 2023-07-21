namespace Honlsoft.Chess; 

public interface IChessBoard {
     
     /// <summary>
     /// Retrieves a square from the chess baard.
     /// </summary>
     /// <param name="squareName">The name of the square.</param>
     /// <returns>The square representation</returns>
     Square GetSquare(SquareName squareName);
     
     /// <summary>
     /// The square a pawn has moved over in the last move. 
     /// </summary>
     SquareName? EnPassant { get; }
}