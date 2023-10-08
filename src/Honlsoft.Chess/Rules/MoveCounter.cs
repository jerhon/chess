using System.Data.SqlTypes;
using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Calculates the  number of moves on the board for a particular color. Helpful for determining if we're in checkmate, or stalemate.
/// </summary>
public class MoveCounter(IChessPosition chessPosition, IEnumerable<IMoveRule> moveRules, PieceColor color) {

    private readonly Dictionary<SquareName, int> _moveCounts = new();
    private int _totalMoves = 0;
    private bool _calculated = false;
    private Square _kingSquare;
    
    /// <summary>
    /// Get the threat count associated with the square.
    /// </summary>
    /// <param name="square"></param>
    /// <returns></returns>
    public int GetThreatCount(SquareName square) {
        CalculateMoves();
        
        if (_moveCounts.TryGetValue(square, out var value)) {
            return value;
        }
        
        return 0;
    }

    public int GetTotalMoves() {
        CalculateMoves();
        
        return _totalMoves;
    }


    public Square GetKingSquare() {
        CalculateMoves();

        return _kingSquare;
    }

    private void CalculateMoves() {
        if (_calculated) {
            return;
        }
        
        foreach (var squareName in SquareName.AllSquares()) {
            var square = chessPosition.GetSquare(squareName);
            if (square?.Piece?.Color == color) {

                if (square is { Piece: { Type: PieceType.King } }) {
                    _kingSquare = square;
                }
                
                // find squares we are moving off of
                foreach (var moveRule in moveRules) {
                    var moves = moveRule.GetCandidateMoves(chessPosition, square.Name).OfType<SimpleMove>();
                    foreach (var move in moves) {
                        if (move.To == squareName)
                            continue;
                        
                        IncreaseSquareMoves(move.To);
                    }
                }
            }
        }

        _calculated = true;
    }

    private void IncreaseSquareMoves(SquareName squareName) {
        if (_moveCounts.TryGetValue(squareName, out var currentValue)) {
            _moveCounts[squareName] = ++currentValue;
        } else {
            _moveCounts[squareName] = 1;
        }
        _totalMoves++;
    }
    
}