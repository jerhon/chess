using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Rules; 

public record CastlingMoveRule(PieceColor Color, CastlingSide Side) : IChessMove, IKingMove {
    
    public SquareName From {
        get {
            SquareRank squareRank = Color is PieceColor.Black ? SquareRank.Rank8 : SquareRank.Rank1;
            SquareName kingSquare = new SquareName(SquareFile.e, squareRank);
            return kingSquare;
        }
    }


    public SquareName To {
        get {
            SquareRank squareRank = Color is PieceColor.Black ? SquareRank.Rank8 : SquareRank.Rank1;
            SquareFile kingFinalSquareFile = Side is CastlingSide.Queenside ? SquareFile.c : SquareFile.g;
        
            SquareName kingToSquare = new SquareName(kingFinalSquareFile, squareRank);
            return kingToSquare;
        }
    }
    
    public void Move(IChessGame chessGame) {
        chessGame.Castle(Side);
    }
    
    public San ToSan() {
        return new SanCastle() { Side = this.Side };
    }

    public PieceColor? GetPlayer(IChessPosition chessPosition) {
        return Color;
    }

    public SquareName[] GetThreateningSquares() {
        return [From, To];
    }

}