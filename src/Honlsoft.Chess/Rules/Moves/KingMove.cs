namespace Honlsoft.Chess.Rules; 

public record KingMove(SquareName From, SquareName To) : IChessMove, IKingMove {
    
    public void Move(IChessGame chessGame) {

        chessGame.Move(From, To, null);
    }
    
    public SquareName[] GetThreateningSquares() {
        return [ To ];
    }
}