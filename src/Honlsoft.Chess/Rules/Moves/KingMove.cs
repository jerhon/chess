namespace Honlsoft.Chess.Rules; 

public record KingMove(SquareName From, SquareName To) : IChessMove, IKingMove {
    
    public void ApplyMove(ChessPositionBuilder chessPosition) {

        var color = chessPosition.GetSquare(From)!.Piece!.Color;
        
        chessPosition.Move(From, To);
        
        chessPosition.WithCastlingRights(color, CastlingSide.Kingside, false);
        chessPosition.WithCastlingRights(color, CastlingSide.Queenside, false);
    }
    
    public SquareName[] GetThreateningSquares() {
        return [ To ];
    }
}