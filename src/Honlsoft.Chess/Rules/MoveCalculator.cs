using System.Data.SqlTypes;
using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Calculates the  number of moves on the board for a particular color. Helpful for determining if we're in checkmate, or stalemate.
/// </summary>
public class MoveCalculator {

    public MoveCalculator(IChessBoard chessBoard, IEnumerable<IMoveRule> moveRules, PieceColor color) {
        _chessBoard = chessBoard;
        _moveRules = moveRules;
        _color = color;
    }

    private readonly IChessBoard _chessBoard;
    private readonly IEnumerable<IMoveRule> _moveRules;
    private readonly PieceColor _color;
    private readonly Dictionary<SquareName, int> _moveCounts = new Dictionary<SquareName, int>();
    private int _totalMoves = 0;
    private bool _calculated = false;
    private Square _kingSquare;
    
    /// <summary>
    /// Get the threat count associated with the square.
    /// </summary>
    /// <param name="square"></param>
    /// <returns></returns>
    public int GetMoveCount(SquareName square) {
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
            var square = _chessBoard.GetSquare(squareName);
            if (square?.Piece?.Color == _color) {

                if (square is { Piece: { Type: PieceType.King } }) {
                    _kingSquare = square;
                }
                
                // find squares we are moving off of
                foreach (var moveRule in _moveRules) {
                    var moves = moveRule.GetPossibleMoves(_chessBoard, square.Name);
                    foreach (var move in moves) {
                        if (move == squareName)
                            continue;
                        
                        IncreaseSquareMoves(move);
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