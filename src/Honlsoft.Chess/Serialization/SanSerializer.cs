using System.Text.RegularExpressions;

namespace Honlsoft.Chess.Serialization;

public class SanSerializer {

    
    private static Regex sanRegex = new Regex("^(?<piece>[KQRBN])?(?<fromFile>[a-h])?(?<fromRank>[1-8])?(?<capture>[x])?(?<toSquare>[a-h][1-8])(?<promotion>[=][QRBN])?(?<check>[+#])?$", RegexOptions.Compiled);


    public string Serialize(San san) {
        if (san is SanCastle sanCastle) {
            return SerializeSanCastle(sanCastle);
        } else if (san is SanMove sanMove) {
            return SerializeSanMove(sanMove);
        }

        throw new InvalidOperationException("Unknown type " + san.GetType().Name);
    }
    
    public string SerializeSanCastle(SanCastle sanCastle) {
        return sanCastle.Side switch {
            CastlingSide.Kingside => "0-0",
            CastlingSide.Queenside => "0-0-0"
        };
    }

    public string SerializeSanFrom(SanMove sanMovePiece) {
        var pieceType = SerializePieceType(sanMovePiece.FromPiece);
        var rank = sanMovePiece.FromRank?.ToString() ?? "";
        var file = sanMovePiece.FromFile?.ToString() ?? "";

        return $"{pieceType}{file}{rank}";
    }

    public string SerializeSanTo(SanMove sanMovePiece) {
        var rank = sanMovePiece.ToRank?.ToString() ?? "";
        var file = sanMovePiece.ToFile?.ToString() ?? "";

        return $"{file}{rank}";
    }

    public string SerializeSanMove(SanMove sanMove) {
        var from = SerializeSanFrom(sanMove);
        var capture = sanMove.Capture ? "x" : "";
        var to = SerializeSanTo(sanMove);
        var check = SerializeCheckType(sanMove.Check);
        var promotion = sanMove.PromotionPiece != null ? "=="  + SerializePieceType(sanMove.PromotionPiece) : "";
        
        return $"{from}{capture}{to}{check}{promotion}";
    }

    public string SerializeCheckType(SanCheckType? checkType) {
        return checkType switch {
            SanCheckType.Check => "+",
            SanCheckType.Checkmate => "#",
            null => ""
        };
    }


    public string SerializePieceType(PieceType? piece) {
        return piece switch {
            PieceType.Bishop => "B",
            PieceType.King => "K",
            PieceType.Knight => "N",
            PieceType.Pawn => "",
            PieceType.Queen => "Q",
            PieceType.Rook => "R",
            null => ""
        };
    }

    
    public San Deserialize(string sanExpression) {

        if (sanExpression is "0-0" or "O-O") {
            return new SanCastle { Side = CastlingSide.Kingside };
        }
        
        if (sanExpression is "0-0-0" or "O-O-O") {
            return new SanCastle { Side = CastlingSide.Queenside };
        }
        
        var matchedSan = sanRegex.Match(sanExpression);
        if (matchedSan.Success) {

            PieceType? pieceType = null;
            SquareRank? fromRank = null;
            SquareFile? fromFile = null;
            
            if (matchedSan.Groups["piece"].Success) {
                pieceType = ParsePiece(matchedSan.Groups["piece"].Value);
            }
            if (matchedSan.Groups["fromFile"].Success) {
                fromFile = SquareFile.Parse(matchedSan.Groups["fromFile"].Value[0]);
            }
            if (matchedSan.Groups["fromRank"].Success) {
                fromRank = SquareRank.Parse(matchedSan.Groups["fromRank"].Value[0]);
            }

            var capture = matchedSan.Groups["capture"].Success;

            SquareFile? toFile = null;
            SquareRank? toRank = null;
            if (matchedSan.Groups["toSquare"].Success) { 
                var toSquare = SquareName.Parse(matchedSan.Groups["toSquare"].Value);
                toFile = toSquare.SquareFile;
                toRank = toSquare.SquareRank;
            }

            SanCheckType? checkType = null;
            if (matchedSan.Groups["check"].Success) {
                var checkValue = matchedSan.Groups["check"].Value;

                if (checkValue == "+") {
                    checkType = SanCheckType.Check;
                }
                if (checkValue == "#") {
                    checkType = SanCheckType.Checkmate;
                }
            }
            
            return new SanMove {
                FromFile = fromFile,
                FromPiece = pieceType,
                FromRank = fromRank,
                ToFile = toFile,
                ToRank = toRank,
                Capture = capture,
                Check = checkType
            };
        }
        
        throw new FormatException($"Does not match a SAN expression = '{sanExpression}'.");
    }


    public PieceType ParsePiece(string pieceType) {
        return pieceType switch {
            "N" => PieceType.Knight,
            "K" => PieceType.King,
            "Q" => PieceType.Queen,
            "B" => PieceType.Bishop,
            "R" => PieceType.Rook,
            _ => throw new FormatException($"{pieceType} is not a piece type.")
        };
    }
}