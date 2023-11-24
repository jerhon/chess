using System.Collections;

namespace Honlsoft.Chess;

public record Square(SquareName Name, Piece? Piece) {
    
    public bool HasPiece => Piece is not null;


    public static Square Parse(string name) {
        
        if (name.Length == 2) {
            var squareName = SquareName.Parse(name);
            return new Square(squareName, null);
        }
        else if (name.Length == 3) {
            var piece = Piece.Parse(name);
            var squareName = SquareName.Parse(name.Substring(1));
            return new Square(squareName, piece);
        }
        
        
        throw new FormatException("Must be an piece abbreviation followed by the square's algebraic notation.  An example is Pd4.");
    }

}
