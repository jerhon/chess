using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Tests; 

public static class ChessBoardUtils {

    public static SquareName[] GetSquares(params string[] squareRepresentation) {

        return squareRepresentation.Select((s) => SquareName.Parse(s)).ToArray();

    }

    public static SimpleMove[] CreateCandidateMoves(string fromSquare, params string[] squareRepresentation) {
        return GetSquares(squareRepresentation).Select((s) => new SimpleMove(SquareName.Parse(fromSquare), s)).ToArray();
    }


    public static IMoveRule[] GetAllMoveRules() {
        return new IMoveRule[] { new PawnMoveRule(), new KnightMoveRule(), new DiagonalMoveRule(), new FileAndRankMoveRule() };
    }
    
}