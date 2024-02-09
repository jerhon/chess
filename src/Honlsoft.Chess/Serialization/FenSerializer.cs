using System.Text;

namespace Honlsoft.Chess.Serialization; 

public class FenSerializer {
    
    public string SerializePiecePositions(IChessPosition chessPosition) {
        StringBuilder builder = new StringBuilder();
        foreach (var rank in SquareRank.AllRanks.Reverse()) {
            int emptySpaces = 0;
            foreach (var file in SquareFile.AllFiles) {
                var square = chessPosition.GetSquare(new SquareName(file, rank));
                if (square.Piece != null) {
                    if (emptySpaces > 0) {
                        builder.Append(emptySpaces);
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
        var castlingState = $"{serializedWhitePieces}{serializedBlackPieces}"; 
    
        if (string.IsNullOrWhiteSpace(castlingState))
        {
            return "-";
        }
        
        return castlingState;
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
        
        if (fenParts.Length > 1) {
            var color = DeserializeColor(fenParts[1]);
            positionBuilder.WithPlayerToMove(color);
        }

        if (fenParts.Length > 2) {
            if (fenParts[2] != "-") {
                DeserializeCastlingRights(positionBuilder, fenParts[2]);
            }
        }

        if (fenParts.Length > 3) {
            if (fenParts[3] != "-") {
                var enPassantSquare = SquareName.Parse(fenParts[3]);
                positionBuilder.WithEnPassantTarget(enPassantSquare);
            }
        }

        if (fenParts.Length > 4) {
            if (int.TryParse(fenParts[4], out int halfMoves)) {
                positionBuilder.WithHalfMoves(halfMoves);
            } else {
                throw new FormatException("The half moves of the FEN string is not a valid integer.");
            }
        }

        if (fenParts.Length > 5) {
            if (int.TryParse(fenParts[5], out int fullMoves)) {
                positionBuilder.WithFullMoves(fullMoves);
            } else {
                throw new FormatException("The half moves of the FEN string is not a valid integer.");
            }
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

    /// <summary>
    /// Deserializes a FEN position string and updates the ChessPositionBuilder object with the corresponding chess position.
    /// </summary>
    /// <param name="builder">The ChessPositionBuilder object to update.</param>
    /// <param name="boardString">The FEN position string representing the chess position.</param>
    /// <exception cref="FormatException">Thrown when the FEN position string does not contain all 8 ranks.</exception>
    /// <remarks>
    /// The FEN position string should follow the standard FEN notation for chess positions.
    /// The method splits the FEN position string into rows and iterates through each row and column to update the ChessPositionBuilder object.
    /// It uses the '//' symbol to split the rows and the letters 'a' to 'h' to represent the columns.
    /// The method handles both piece characters and numeric characters to update the corresponding chess squares.
    /// </remarks>
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
                    file += (char)(int.Parse(currentLetter.ToString()) - 1);
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
            'n' => PieceType.Knight,
            'r' => PieceType.Rook,
            'p' => PieceType.Pawn,
            _ => throw new NotImplementedException("Unknown chess piece representation " + c)
        };

        var pieceColor = Char.IsLower(c) ? PieceColor.Black : PieceColor.White;

        return new Piece(pieceType, pieceColor);
    }
    
    public static FenSerializer Default { get; } = new FenSerializer();
}