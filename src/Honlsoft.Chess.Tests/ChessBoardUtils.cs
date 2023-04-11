using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests; 

public static class ChessBoardUtils {

    public static SquareName[] GetSquares(params string[] squareRepresentation) {

        return squareRepresentation.Select((s) => SquareName.Parse(s)).ToArray();

    }

}