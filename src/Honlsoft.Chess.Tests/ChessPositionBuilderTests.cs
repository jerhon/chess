namespace Honlsoft.Chess.Tests; 

public class ChessPositionBuilderTests {


    [Fact]
    public void WithSquare_AddsSquare_BuildsChessBoard() {

        var chessBoardBuilder = new ChessPositionBuilder();

        chessBoardBuilder
            .SetSquare("Pb4")
            .SetSquare("pc4");

        var chessBoard = chessBoardBuilder.Build();

        var squarePb4 = chessBoard.GetSquare(SquareName.Parse("b4"));
        var squarepc4 = chessBoard.GetSquare(SquareName.Parse("c4"));
        var squareg5 = chessBoard.GetSquare(SquareName.Parse("g5"));
        
        squarePb4.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("Pb4"));
        squarepc4.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("pc4"));
        squareg5.Should().NotBeNull().And.BeEquivalentTo(Square.Parse("g5"));

    }
    
    

    [Fact]
    public void StandardGame_MatchesExpected() 
    {
        var chessBoard = ChessPositionBuilder.StandardGame;
        foreach (var file in SquareFile.AllFiles) {
            Square square = chessBoard.GetSquare(new SquareName(file, SquareRank.Rank7));
            Assert.Equal(PieceType.Pawn, square!.Piece!.Type);
            Assert.Equal(PieceColor.Black, square!.Piece!.Color);
            
            
            square = chessBoard.GetSquare(new SquareName(file, SquareRank.Rank2));
            Assert.Equal(PieceType.Pawn, square!.Piece!.Type);
            Assert.Equal(PieceColor.White, square!.Piece!.Color);
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
        
        Assert.Null(chessBoard.EnPassantTarget);
        
        // Check all the pieces in the middle

        foreach (var file in SquareFile.AllFiles) {
            foreach (var rank in new[] { SquareRank.Rank3, SquareRank.Rank4, SquareRank.Rank5, SquareRank.Rank6 }) {
                var square = chessBoard.GetSquare(new SquareName(file, rank));
                
                Assert.Null(square.Piece);
            }
        }

        Assert.True( chessBoard.CanCastle(PieceColor.Black, CastlingSide.Kingside));
        Assert.True( chessBoard.CanCastle(PieceColor.Black, CastlingSide.Queenside));
        Assert.True( chessBoard.CanCastle(PieceColor.White, CastlingSide.Kingside));
        Assert.True( chessBoard.CanCastle(PieceColor.White, CastlingSide.Queenside));
    }
    
    
    private void AssertPiece(IChessPosition chessPosition, string position, PieceType type, PieceColor color) {
        if (SquareName.TryParse(position, null, out var squareName)) {
            Square square = chessPosition.GetSquare(squareName!);
            Assert.Equal(type, square!.Piece!.Type);
            Assert.Equal(color, square!.Piece!.Color);
        } else {
            Assert.Fail("Invalid chess position");
        }
    }
}