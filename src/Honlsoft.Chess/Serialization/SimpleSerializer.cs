using System.Text;

namespace Honlsoft.Chess.Serialization; 


/// <summary>
/// Provides a simple serializer used mainly for internal testing.  It concatenates all the squares on a chess board together in a format such as &quot;Pa2:Pb2&quot;
/// </summary>
public class SimpleSerializer {

    public char DividerCharacter { get; } = ':';
    
    public string Serialize(ChessBoard chessBoard) {
        StringBuilder sb = new StringBuilder();
        foreach (var file in File.AllFiles) {
            foreach (var rank in Rank.AllRanks) {
                var square = chessBoard.GetSquare(new SquareName(file, rank));
                if (square?.Piece != null) {
                    sb.Append(square.Piece);
                    sb.Append(square.Name);
                    sb.Append(DividerCharacter);
                }
            }
        }
        if (sb.Length > 0) {
            sb.Length = sb.Length - 1;
        }
        return sb.ToString();
    }

    public ChessBoard Deserialize(string serializedChessBoard) {

        var chessBoard = new ChessBoard();
        var squareStrings = serializedChessBoard.Split(DividerCharacter);
        foreach (var squareString in squareStrings) {
            var piece = Piece.Parse(squareString);
            var squareName = SquareName.Parse(squareString.Substring(1));
            chessBoard = chessBoard.SetPiece(squareName, piece);
        }
        return chessBoard;
    }
}