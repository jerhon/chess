using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

public class MoveRules(IEnumerable<IMoveRule> moveRules) {

    public IEnumerable<IChessMove> GetMoves(IChessPosition chessPosition, SquareName from) {
        
        var square = chessPosition.GetSquare(from);
        if (square.Piece == null) {
            return [];
        }
        
        var moves = moveRules
            .Where(movementRule => movementRule.IsApplicable(chessPosition, from))
            .SelectMany(movementRule => movementRule.GetCandidateMoves(chessPosition, from))
            .Distinct();
        
        // We need to evaluate the king specially, because it can't move into a threat, which is unique to it.
        if (square.Piece.Type == PieceType.King) {
            var threatCounter = GetMoveCounter(chessPosition, Piece.GetOppositeColor(square.Piece.Color));
            moves = moves.Where((move) => !IsThreatened(threatCounter, move.From, move.To));
        }

        return moves;
    }

    public IChessMove? GetMove(IChessPosition chessPosition, SquareName from, SquareName to) {
        var moves = GetMoves(chessPosition, from);
        return moves.FirstOrDefault((move) => move.To == to);
    }

    public bool IsThreatened(MoveCounter threats, SquareName from, SquareName to)
    {
        // We need to evaluate the king specially, because it can't move into a threat, which is unique to it.
        // Enumerate all the squares between from and to, and check if they are threatened.
        foreach (var file in from.SquareFile.To(to.SquareFile, true))
        {
            foreach (var rank in from.SquareRank.To(to.SquareRank, true))
            {
                if (threats.GetMoveCount(new SquareName(file, rank)) > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public MoveCounter GetMoveCounter(IChessPosition chessPosition, PieceColor player) {
        return new MoveCounter(chessPosition, moveRules, player);
    }
    
}