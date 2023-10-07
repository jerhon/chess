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
            PieceType.Rook => "r",
            _ => throw new NotImplementedException("Does not implement piece type.")
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

    public static Piece Parse(string value) {
        if (value.Length < 1) {
            throw new FormatException("String must have at least one character.");
        }

        var pieceType = ParsePieceType(value);
        var pieceColor = Char.IsUpper(value[0]) ? PieceColor.White : PieceColor.Black;

        return new Piece(pieceType, pieceColor);
    }

    public static PieceType ParsePieceType(string value) {
        char pieceChar = value[0];
        pieceChar = Char.ToLower(pieceChar);

        var pieceType = pieceChar switch {
            'p' => PieceType.Pawn,
            'b' => PieceType.Bishop,
            'k' => PieceType.King,
            'n' => PieceType.Knight,
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            _ => throw new FormatException("Unrecognized piece type.")
        };

        return pieceType;
    }
}