namespace Honlsoft.Chess; 

public enum MoveResult {
    ValidMove,
    GameOver,
    PieceWrongColor,
    MustMoveOutOfCheck,
    NotALegalMove,
    RequiresPromotion,
    CastlingNotAllowed,
    NotAValidPromotion,
}