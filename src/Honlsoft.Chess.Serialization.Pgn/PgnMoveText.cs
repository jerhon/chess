using Honlsoft.Chess;

namespace Honlsoft.Chess.Serialization.Pgn;

/// <summary>
/// Text associated with a move.
/// </summary>
public record PgnMoveText(PieceType? Piece, SquareFile? FirstFile, SquareRank? FirstRank, bool IsCapture, SquareFile? SecondFile, SquareRank? SecondRank, PieceType? PromotionPiece, bool IsCheck, bool IsCheckMate, string? Annotation) : PgnMovePart {
    
    

    /// <summary>
    /// Parses the chess piece type using SAN syntax.
    /// </summary>
    /// <param name="pieceChar">The character representing the piece.</param>
    /// <returns>The type of piece.</returns>
    public static PieceType? ParsePieceType(char pieceChar) {
        switch (char.ToUpperInvariant(pieceChar)) {
            case 'K': 
                return PieceType.King;
            case 'Q': 
                return PieceType.Queen;
            case 'B':
                return PieceType.Bishop;
            case 'N':
                return PieceType.Knight;
            case 'R':
                return PieceType.Rook;
            case 'P':
                return PieceType.Pawn;
            default:
                return null;
        };
    }
    
    public static PgnMoveText Parse(string moveText) {

        // First try to get the piece type
        int idx = 0;
        PieceType? pieceType = null;
        if (moveText.Length > idx) {
            pieceType = ParsePieceType(moveText[0]);
        }
        if (pieceType != null) {
            idx++;
        }
        
        // Folowed by a file / rank
        SquareFile? firstFile = null;
        if (moveText.Length > idx) {
            firstFile = SquareFile.Parse(moveText[idx]);
        }
        if (firstFile != null) {
            idx++;
        }

        SquareRank? firstRank = null;
        if (moveText.Length > idx) {
            firstRank = SquareRank.Parse(moveText[idx]);
        }
        if (firstRank != null) {
            idx++;
        }


        bool isCapture = false;
        
        if (moveText.Length > idx && moveText[idx] == 'x') {
            isCapture = true;
            idx++;
        }
        
        // Folowed by a file / rank
        SquareFile? secondFile = null;
        if (moveText.Length > idx) {
            secondFile = SquareFile.Parse(moveText[idx]);
        }
        if (secondFile != null) {
            idx++;
        }
        
        // Folowed by a file / rank
        SquareRank? secondRank = null;
        if (moveText.Length > idx) {
            secondRank = SquareRank.Parse(moveText[idx]);
        }
        if (secondRank != null) {
            idx++;
        }

        // Promotion
        PieceType? promotionPiece = null;
        if (moveText.Length > idx && moveText[idx] == '=') {
            idx++;
            
            // A piece text is required always?
            if (moveText.Length > idx) {
                promotionPiece = ParsePieceType(moveText[idx]);
            }
        }

        bool isCheck = false;
        bool isCheckmate = false;
        // Followed by a check or a checkmate

        if (moveText.Length > idx && moveText[idx] == '+') {
            isCheck = true;
            idx++;
        }


        if (moveText.Length > idx &&moveText[idx] == '#') {
            isCheckmate = true;
            idx++;
        }

        // The rest of the text is the annotation
        var annotation = moveText.Substring(idx);

        return new PgnMoveText(pieceType, firstFile, firstRank, isCapture, secondFile, secondRank, promotionPiece, isCheck, isCheckmate, annotation);

    }
}