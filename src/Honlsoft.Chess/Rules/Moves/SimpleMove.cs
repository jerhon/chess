using Honlsoft.Chess.Serialization;

namespace Honlsoft.Chess.Rules;

/// <summary>
/// Moves a piece from one square to another.
/// </summary>
/// <param name="from">The square to move from.</param>
/// <param name="to">The square to move to.</param>
public record SimpleMove(SquareName From, SquareName To) : IChessMove {

    public void Move(IChessGame chessGame) {
        
        chessGame.Move(From, To, null);
        
    }
    public San ToSan() {
        return new SanMove{ FromFile = From.SquareFile, FromRank = From.SquareRank, ToFile = To.SquareFile, ToRank = To.SquareRank };
    }

    public SquareName[] GetTargetedSquares() {
        return [To];
    }
    
    public PieceColor? GetPlayer(IChessPosition chessPosition) {
        return chessPosition.GetSquare(From)?.Piece?.Color;
    }

}