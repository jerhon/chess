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

    public IChessPosition Deserialize(string fenString) {
        ChessPositionBuilder positionBuilder = new ChessPositionBuilder();
        var fenParts = fenString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (fenParts.Length > 0) {
            DeserializePieces(positionBuilder, fenParts[0]);
        }
        
        // rnbqkbnr/2p2p2/1p4p1/p2pp2p/P2PP2P/1P4P1/2P2P2/RNBQKBNR w KQkq - 0 1

        if (fenParts.Length > 1) {
            var color = DeserializeColor(fenParts[1]);
            positionBuilder.WithPlayerToMove(color);
        }

        if (fenParts.Length > 2) {
            if (fenParts[2] != "-") {
                DeserializeCastlingRights(positionBuilder, fenParts[1]);
            }
        }

        if (fenParts.Length > 3) {
            if (fenParts[3] != "-") {
                var enPassantSquare = SquareName.Parse(fenParts[3]);
                positionBuilder.WithEnPassantTarget(enPassantSquare);
            }
        }

        if (fenParts.Length > 4) {
            int halfMoves = int.Parse(fenParts[4]);
            positionBuilder.WithHalfMoves(halfMoves);
        }

        if (fenParts.Length > 5) {
            int fullMoves = int.Parse(fenParts[5]);
            positionBuilder.WithFullMoves(fullMoves);
        }
        
        return positionBuilder;
    }

    public PieceColor DeserializeColor(string color) {
        return color == "w" ? PieceColor.White : PieceColor.Black;
    }

    public void DeserializeCastlingRights(ChessPositionBuilder positionBuilder, string castlingRights) {
        foreach (var c in castlingRights) {
            if (c == 'K') {
                positionBuilder.WithCastlingRights(PieceColor.White, CastlingSide.Kingside, true);
            } else if (c == 'Q') {
                positionBuilder.WithCastlingRights(PieceColor.White, CastlingSide.Queenside, true);
            } else if (c == 'k') {
                positionBuilder.WithCastlingRights(PieceColor.Black, CastlingSide.Kingside, true);
            } else if (c == 'q') {
                positionBuilder.WithCastlingRights(PieceColor.Black, CastlingSide.Queenside, true);
            }
        }
    }

    public void DeserializePieces(ChessPositionBuilder builder, string boardString) {
        var rows = boardString.Split("/");
        if (rows.Length != 8) {
            throw new FormatException("FEN position string does not contain all 8 ranks.");
        }
        
        for (int rank = 1; rank <= 8; rank++) {
            string currentRow = rows[8 - rank];
            int currentLetterIdx = 0;
            for (char file = 'a'; file <= 'h'; file++) {
                if (currentLetterIdx >= currentRow.Length) {
                    break;
                }
                char currentLetter = currentRow[currentLetterIdx];
                if (Char.IsNumber(currentLetter)) {
                    file += (char)int.Parse(currentLetter.ToString());
                } else {
                    var piece = CharToPiece(currentLetter);
                    builder.SetSquare(SquareName.From(file, rank), piece);
                }
                currentLetterIdx++;
            }
        }
    }


    private static Piece CharToPiece(char c) {

        var pieceType = Char.ToLower(c) switch {
            'k' => PieceType.King,
            'q' => PieceType.Queen,
            'b' => PieceType.Bishop,
            'n' => PieceType.King,
            'r' => PieceType.Rook,
            'p' => PieceType.Pawn,
            _ => throw new NotImplementedException("Unknown piece character " + c)
        };

        var pieceColor = Char.IsLower(c) ? PieceColor.Black : PieceColor.White;

        return new Piece(pieceType, pieceColor);
    }
}