namespace Honlsoft.Chess; 


public enum PieceType {
    King,
    Queen,
    Bishop,
    Knight,
    Rook,
    Pawn
}

public enum PieceColor {
    White,
    Black
}

/// <summary>
/// Represents a single chess piece.
/// </summary>
/// <param name="Type">The type of piece.</param>
/// <param name="Color">The color of the piece.</param>
public record Piece(PieceType Type, PieceColor Color) {

    public override string ToString() {
        var pieceChar = Type switch {
            PieceType.Pawn => "p",
            PieceType.Bishop => "b",
            PieceType.King => "k",
            PieceType.Knight => "n",
            PieceType.Queen => "q",
            PieceType.Rook => "r"
        };

        if (Color == PieceColor.White) {
            return pieceChar.ToUpper();
        } else {
            return pieceChar;
        }
    }


    public bool IsOpponent(PieceColor color) {
        return this.Color != color;
    }
}