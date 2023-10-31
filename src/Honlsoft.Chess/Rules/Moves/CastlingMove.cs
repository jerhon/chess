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


    public void ApplyMove(ChessPositionBuilder chessPosition) {

        if (chessPosition.CanCastle(Color, Side)) {
            throw new InvalidOperationException("Trying to castle when it's not allowed.");
        }

        SquareRank squareRank = Color is PieceColor.Black ? SquareRank.Rank8 : SquareRank.Rank1;
        SquareFile rookSquareFile = Side is CastlingSide.Queenside ? SquareFile.a : SquareFile.h;

        SquareFile rookFinalSquareFile = Side is CastlingSide.Queenside ? SquareFile.d : SquareFile.f;
        SquareFile kingFinalSquareFile = Side is CastlingSide.Queenside ? SquareFile.c : SquareFile.g;
        
        SquareName kingSquare = new SquareName(SquareFile.e, squareRank);
        SquareName rookSquare = new SquareName(rookSquareFile, squareRank);
        SquareName kingToSquare = new SquareName(kingFinalSquareFile, squareRank);
        SquareName rookToSquare = new SquareName(rookFinalSquareFile, squareRank);
        
        chessPosition.Move(kingSquare, kingToSquare);
        chessPosition.Move(rookSquare, rookToSquare);
    }
    
    public PieceColor? GetPlayer(IChessPosition chessPosition) {
        return Color;
    }

    public SquareName[] GetThreateningSquares() {
        return [From, To];
    }
}