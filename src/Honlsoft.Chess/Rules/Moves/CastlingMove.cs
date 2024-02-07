using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Rules; 

public record CastlingMoveRule(PieceColor Color, CastlingSide Side) : IChessMove {
    
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

    public void Move(ChessPositionBuilder chessGame)
    {
        if (Side == CastlingSide.Kingside)
        {
            chessGame.Move(new SquareName(SquareFile.e, From.SquareRank), new SquareName(SquareFile.g, From.SquareRank));
            chessGame.Move(new SquareName(SquareFile.h, From.SquareRank), new SquareName(SquareFile.f, From.SquareRank));
        }
        else
        {
            chessGame.Move(new SquareName(SquareFile.e, From.SquareRank), new SquareName(SquareFile.c, From.SquareRank));
            chessGame.Move(new SquareName(SquareFile.a, From.SquareRank), new SquareName(SquareFile.d, From.SquareRank));
        }
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