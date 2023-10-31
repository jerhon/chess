using System.Text;

namespace Honlsoft.Chess.Serialization; 

public class FenSerializer {
    
    public string Serialize(IChessPosition chessPosition) {
        StringBuilder builder = new StringBuilder();
        foreach (var rank in SquareRank.Rank1.ToEnd(true).Reverse()) {
            int emptySpaces = 0;
            foreach (var file in SquareFile.a.ToEnd(true)) {
                var square = chessPosition.GetSquare(new SquareName(file, rank));
                if (square.Piece != null) {
                    if (emptySpaces > 0) {
                        builder.Append(emptySpaces.ToString());
                        emptySpaces = 0;
                    }
                    builder.Append(square.Piece.ToString());
                } else {
                    emptySpaces++;
                }
            }
            if (emptySpaces > 0) {
                builder.Append(emptySpaces.ToString());
            }
            builder.Append("/");
        }
        builder.Length -= 1;
        return builder.ToString();
    }
    
    public string Serialize(IChessGame chessGame) {
        var positions = Serialize(chessGame);

        var moveTurn = chessGame.CurrentPlayer == PieceColor.Black ? "b" : "w";
        

        return $"{positions} {moveTurn}";

    }
}