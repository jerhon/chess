using System.Text;

namespace Honlsoft.Chess.Serialization; 


/// <summary>
/// Provides a simple serializer used mainly for internal testing.  It concatenates all the squares on a chess board together in a format such as &quot;Pa2:Pb2&quot;
/// </summary>
public class SimpleSerializer {

    public char DividerCharacter { get; } = ':';
    
    public string Serialize(IChessPosition chessPosition) {
        StringBuilder sb = new StringBuilder();
        foreach (var file in SquareFile.AllFiles) {
            foreach (var rank in SquareRank.AllRanks) {
                var square = chessPosition.GetSquare(new SquareName(file, rank));
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

    public IChessPosition Deserialize(string serializedChessBoard) {
        var chessBoardBuilder = new ChessPositionBuilder();
        var squareStrings = serializedChessBoard.Split(DividerCharacter);
        foreach (var squareString in squareStrings) {
            var square = Square.Parse(squareString);
            chessBoardBuilder.SetSquare(square);
        }
        return chessBoardBuilder.Build();
    }

    public static SanSerializer Default { get; } = new SanSerializer();
}