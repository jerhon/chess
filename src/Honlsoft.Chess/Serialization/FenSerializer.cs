using System.Text;

namespace Honlsoft.Chess.Serialization; 

public class FenSerializer {
    
    public string SerializePiecePositions(IChessPosition chessPosition) {
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
        return SerializeEmptyString(builder.ToString());
    }
    
    public string Serialize(IChessPosition chessPosition) {
        var positions = SerializePiecePositions(chessPosition);
        var moveTurn = SerializePlayerToMove(chessPosition);
        var castling = SerializeCastlingState(chessPosition);
        var enPassant = SerializeEnPassantTarget(chessPosition);
        var halfMoves = chessPosition.HalfMoves.ToString();
        var fullMoves = chessPosition.FullMoves.ToString();

        return $"{positions} {moveTurn} {castling} {enPassant} {halfMoves} {fullMoves}";

    }

    public string SerializePlayerToMove(IChessPosition chessPosition) => chessPosition.PlayerToMove == PieceColor.Black ? "b" : "w";

    public string SerializeEnPassantTarget(IChessPosition chessPosition) {
        return SerializeEmptyString( chessPosition.EnPassantTarget?.ToString());
    }

    public string SerializeCastlingState(IChessPosition chessPosition) 
    {
        var serializedWhitePieces = SerializePieceCastlingState(PieceColor.White, chessPosition);
        var serializedBlackPieces = SerializePieceCastlingState(PieceColor.Black, chessPosition);
    
        return $"{serializedWhitePieces}{serializedBlackPieces}";
    }

    private string SerializePieceCastlingState(PieceColor color, IChessPosition chessPosition)
    {
        var kingSide = chessPosition.CanCastle(color, CastlingSide.Kingside) ? (color == PieceColor.White ? "K" : "k") : "";
        var queenSide = chessPosition.CanCastle(color, CastlingSide.Queenside) ? (color == PieceColor.White ? "Q" : "q") : "";
    
        return $"{kingSide}{queenSide}";
    }

    public string SerializeEmptyString(string value) {
        if (string.IsNullOrEmpty(value)) {
            return "-";
        }
        return value;
    }
}