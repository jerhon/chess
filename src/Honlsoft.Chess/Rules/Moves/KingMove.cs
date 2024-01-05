using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Rules; 

public record KingMove(SquareName From, SquareName To) : IChessMove, IKingMove {
    
    public void Move(IChessGame chessGame) {

        chessGame.Move(From, To, null);
    }
    public San ToSan() {
        return new SanMove{ FromFile = From.SquareFile, FromRank = From.SquareRank, ToFile = To.SquareFile, ToRank = To.SquareRank };
    }

    public SquareName[] GetThreateningSquares() {
        return [ To ];
    }
}