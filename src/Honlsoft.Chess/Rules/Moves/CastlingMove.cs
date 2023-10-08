namespace Honlsoft.Chess.Rules; 

public record CastlingMoveRule(PieceColor Color, CastlingSide Side) : IChessMove, IKingMove {



    public SquareName From {
        get {
            Rank rank = Color is PieceColor.Black ? Rank.Rank8 : Rank.Rank1;
            SquareName kingSquare = new SquareName(File.e, rank);
            return kingSquare;
        }
    }


    public SquareName To {
        get {
            Rank rank = Color is PieceColor.Black ? Rank.Rank8 : Rank.Rank1;
            File kingFinalFile = Side is CastlingSide.Queenside ? File.c : File.g;
        
            SquareName kingToSquare = new SquareName(kingFinalFile, rank);
            return kingToSquare;
        }
    }


    public void ApplyMove(ChessPositionBuilder chessPosition) {

        if (chessPosition.CanCastle(Color, Side)) {
            throw new InvalidOperationException("Trying to castle when it's not allowed.");
        }

        Rank rank = Color is PieceColor.Black ? Rank.Rank8 : Rank.Rank1;
        File rookFile = Side is CastlingSide.Queenside ? File.a : File.h;

        File rookFinalFile = Side is CastlingSide.Queenside ? File.d : File.f;
        File kingFinalFile = Side is CastlingSide.Queenside ? File.c : File.g;
        
        SquareName kingSquare = new SquareName(File.e, rank);
        SquareName rookSquare = new SquareName(rookFile, rank);
        SquareName kingToSquare = new SquareName(kingFinalFile, rank);
        SquareName rookToSquare = new SquareName(rookFinalFile, rank);
        
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