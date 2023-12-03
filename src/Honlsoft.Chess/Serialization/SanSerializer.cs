using System.Text.RegularExpressions;

namespace Honlsoft.Chess.Serialization;

public class SanSerializer {

    
    private static Regex sanRegex = new Regex("^(?<piece>[KQRBN])?(?<fromFile>[a-h])?(?<fromRank>[1-8])?(?<capture>[x])?(?<toSquare>[a-h][1-8])?(?<promotion>[=][QRBN])?(?<check>[+#])?$", RegexOptions.Compiled);


    public string SerializeSanFrom(San sanPiece) {
        var pieceType = SerializePieceType(sanPiece.FromPiece);
        var rank = sanPiece.FromRank?.ToString() ?? "";
        var file = sanPiece.FromFile?.ToString() ?? "";

        return $"{pieceType}{rank}{file}";
    }

    public string SerializeSanTo(San sanPiece) {
        var rank = sanPiece.ToRank?.ToString() ?? "";
        var file = sanPiece.ToRank?.ToString() ?? "";

        return $"{rank}{file}";
    }

    public string SerializeSan(San san) {
        var from = SerializeSanFrom(san);
        var capture = san.Capture ? "x" : "";
        var to = SerializeSanTo(san);
        var check = SerializeCheckType(san.Check);
        var promotion = san.PromotionPiece != null ? "=="  + SerializePieceType(san.PromotionPiece) : "";
        
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
            
            return new San {
                FromFile = fromFile,
                FromPiece = pieceType,
                FromRank = fromRank,
                ToFile = toFile,
                ToRank = toRank,
                Capture = capture,
                Check = checkType
            };

        }
        else 
        {
            throw new FormatException("Does not match a SAN expression.");
        }
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