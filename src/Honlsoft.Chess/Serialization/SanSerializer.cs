﻿using System.Text.RegularExpressions;

namespace Honlsoft.Chess.Serialization;

public class SanSerializer {

    
    private static readonly Regex sanRegex = new Regex("^(?<piece>[KQRBN])?(?<fromFile>[a-h])?(?<fromRank>[1-8])?(?<capture>[x])?(?<toSquare>[a-h][1-8])(?<promotion>[=]+[QRBN])?(?<check>[+#])?$", RegexOptions.Compiled);


    public string Serialize(San san) {
        if (san is SanCastle sanCastle) {
            return SerializeSanCastle(sanCastle);
        } else if (san is SanMove sanMove) {
            return SerializeSanMove(sanMove);
        }

        throw new InvalidOperationException("Unknown type " + san.GetType().Name);
    }
    
    public string SerializeSanCastle(SanCastle sanCastle) {
        var castle = sanCastle.Side switch {
            CastlingSide.Kingside => "O-O",
            CastlingSide.Queenside => "O-O-O"
        };
        
        return castle + SerializeCheckType(sanCastle.Check);
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
    
    public SanCheckType? DeserializeCheckType(string checkType) {
        return checkType switch {
            "+" => SanCheckType.Check,
            "#" => SanCheckType.Checkmate,
            _ => null
        };
    }
    
    public San Deserialize(string sanExpression) {
        if (sanExpression.StartsWith("0-0-0") || sanExpression.StartsWith( "O-O-O")) {
            var postfix = sanExpression.Substring(5);
            var checkType = DeserializeCheckType(postfix);
            
            return new SanCastle { Side = CastlingSide.Queenside, Check = checkType };
        }
        if (sanExpression.StartsWith( "0-0" ) || sanExpression.StartsWith("O-O")) {
              
              var postfix = sanExpression.Substring(3);
              var checkType = DeserializeCheckType(postfix);
              
              return new SanCastle { Side = CastlingSide.Kingside, Check = checkType };
        }      
        var matchedSan = sanRegex.Match(sanExpression);
        if (matchedSan.Success) {

            PieceType? pieceType = null;
            SquareRank? fromRank = null;
            SquareFile? fromFile = null;
            PieceType? promotionPiece = null;
            
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

            if (matchedSan.Groups["promotion"].Success) {
                var promotionValue = matchedSan.Groups["promotion"].Value.TrimStart('=');
                promotionPiece = ParsePiece(promotionValue);
            }
            
            return new SanMove {
                FromFile = fromFile,
                FromPiece = pieceType,
                FromRank = fromRank,
                ToFile = toFile,
                ToRank = toRank,
                Capture = capture,
                Check = checkType,
                PromotionPiece = promotionPiece
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