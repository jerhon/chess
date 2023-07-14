namespace Honlsoft.Chess.Rules; 

public class MoveRulesEngine {

    private readonly IMoveRule[] _moveRules;
    
    public MoveRulesEngine(IMoveRule[] moveRule) {
        _moveRules = moveRule;
    }
    
    public IEnumerable<SquareName> GetCandidateMoves(IChessBoard chessBoard, SquareName from) {
        foreach (var movementRule in _moveRules) {
            if (movementRule.IsApplicable(chessBoard, from)) {
                var movements = movementRule.GetPossibleMoves(chessBoard, from);
                foreach (var move in movements) {
                    yield return move;
                }
            }
        }
    }

}