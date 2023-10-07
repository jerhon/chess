namespace Honlsoft.Chess; 

public enum MoveResult {
    ValidMove,
    GameOver,
    PieceWrongColor,
    InCheckMustMoveKing,
    NotALegalMove,
}