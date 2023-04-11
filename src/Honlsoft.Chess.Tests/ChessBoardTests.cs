using System.Net.NetworkInformation;

namespace Honlsoft.Chess.Tests; 

public class ChessBoardTests {
    
    
    [Fact]
    public void ChessBoard_Empty() {
        var chessBoard = new ChessBoard();

        foreach (var square in chessBoard) {
            Assert.Null(square.Piece);
        }
    }

    [Fact]
    public void NewStandardGame_MatchesExpected() 
    {
        var chessBoard = ChessBoard.NewStandardGame();
        foreach (var file in File.AllFiles) {
            Square square = chessBoard.GetSquare(new SquareName(file, Rank.Rank7));
            Assert.Equal(PieceType.Pawn, square.Piece.Type);
            Assert.Equal(PieceColor.Black, square.Piece.Color);
            
            
            square = chessBoard.GetSquare(new SquareName(file, Rank.Rank2));
            Assert.Equal(PieceType.Pawn, square.Piece.Type);
            Assert.Equal(PieceColor.White, square.Piece.Color);
        }
    
        AssertPiece(chessBoard, "a1", PieceType.Rook, PieceColor.White);
        AssertPiece(chessBoard, "b1", PieceType.Knight, PieceColor.White);
        AssertPiece(chessBoard, "c1", PieceType.Bishop, PieceColor.White);
        AssertPiece(chessBoard, "d1", PieceType.Queen, PieceColor.White);
        AssertPiece(chessBoard, "e1", PieceType.King, PieceColor.White);
        AssertPiece(chessBoard, "f1", PieceType.Bishop, PieceColor.White);
        AssertPiece(chessBoard, "g1", PieceType.Knight, PieceColor.White);
        AssertPiece(chessBoard, "h1", PieceType.Rook, PieceColor.White);
        
        AssertPiece(chessBoard, "a8", PieceType.Rook, PieceColor.Black);
        AssertPiece(chessBoard, "b8", PieceType.Knight, PieceColor.Black);
        AssertPiece(chessBoard, "c8", PieceType.Bishop, PieceColor.Black);
        AssertPiece(chessBoard, "d8", PieceType.Queen, PieceColor.Black);
        AssertPiece(chessBoard, "e8", PieceType.King, PieceColor.Black);
        AssertPiece(chessBoard, "f8", PieceType.Bishop, PieceColor.Black);
        AssertPiece(chessBoard, "g8", PieceType.Knight, PieceColor.Black);
        AssertPiece(chessBoard, "h8", PieceType.Rook, PieceColor.Black);
        
        // Check all the pieces in the middle

        foreach (var file in File.AllFiles) {
            foreach (var rank in new[] { Rank.Rank3, Rank.Rank4, Rank.Rank5, Rank.Rank6 }) {
                var square = chessBoard.GetSquare(new SquareName(file, rank));
                
                Assert.Null(square.Piece);
            }
        }
    }
    
    
    private void AssertPiece(ChessBoard chessBoard, string position, PieceType type, PieceColor color) {
        if (SquareName.TryParse(position, null, out var squareName)) {
            Square square = chessBoard.GetSquare(squareName);
            Assert.Equal(type, square.Piece.Type);
            Assert.Equal(color, square.Piece.Color);
        } else {
            Assert.Fail("Invalid chess position");
        }
    }

}