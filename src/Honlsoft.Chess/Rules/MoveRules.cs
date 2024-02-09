using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

public class MoveRules(IEnumerable<IMoveRule> moveRules) {
    
    public IEnumerable<IChessMove> GetMoves(IChessPosition chessPosition, PieceColor colors)
    {
        return SquareName.AllSquares()
            .Select((s) => chessPosition.GetSquare(s))
            .Where((square) => square.Piece?.Color == colors)
            .SelectMany((square) => GetMoves(chessPosition, square.Name));
    }
    
    public IEnumerable<IChessMove> GetMoves(IChessPosition chessPosition, SquareName from) {
        
        var square = chessPosition.GetSquare(from);
        if (square.Piece == null) {
            return [];
        }
        
        var moves = moveRules
            .Where(movementRule => movementRule.IsApplicable(chessPosition, from))
            .SelectMany(movementRule => movementRule.GetCandidateMoves(chessPosition, from))
            .Distinct();
        
        // We need to evaluate the king specially, because it can't move into check
        if (square.Piece.Type == PieceType.King) {
            moves = moves.Where((move) => !IsThreatened(chessPosition, move.From, move.To));
        }
        else {
            
            var kingSquare = chessPosition.GetKingSquare(square.Piece.Color);
            var initialBuilder = new ChessPositionBuilder().FromPosition(chessPosition);
            
            // Need to remove any moves that if made would put the king in check.
            moves = moves.Where((move) => {
                var newPosition = initialBuilder
                    .Clone()
                    .Move(move);
                
                return !IsKingInCheck(newPosition, kingSquare);
            });
        }

        return moves;
    }

    public IChessMove? GetMove(IChessPosition chessPosition, SquareName from, SquareName to) {
        var moves = GetMoves(chessPosition, from);
        return moves.FirstOrDefault((move) => move.To == to);
    }

    public bool IsKingInCheck(IChessPosition position, SquareName kingSquare) {
        MoveCounter opponentMoves = GetMoveCounter(position, Piece.GetOppositeColor(position.PlayerToMove));
        return opponentMoves.GetMoveCount(kingSquare) > 0;
    }

    public bool IsThreatened(IChessPosition initialPosition, SquareName from, SquareName to)
    {
        var opponent = Piece.GetOppositeColor(initialPosition.GetSquare(from).Piece.Color); 
        
        var chessPosition = new ChessPositionBuilder()
                            .FromPosition(initialPosition)
                            .Move(from, to);

        var threats = GetMoveCounter(chessPosition, opponent); 
        
        // Check the initial threat in the position.
        if (threats.GetMoveCount(to) > 0) {
            return true;
        }
        
        // if this is a move with more than one square, it's a castle and we need to check in between squares.
        if (from.SquareRank == to.SquareRank && Math.Abs(to.SquareFile.Index - from.SquareFile.Index) > 1)
        {
            foreach (var file in from.SquareFile.To(to.SquareFile))
            {
                var newSquare = new SquareName(file, from.SquareRank);
                
                chessPosition = new ChessPositionBuilder()
                    .FromPosition(initialPosition)
                    .Move(from, newSquare);
                
                threats = GetMoveCounter(chessPosition, opponent);
                
                if (threats.GetMoveCount(new SquareName(file, from.SquareRank)) > 0)
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