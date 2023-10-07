using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

public class ChessGameFactory {
    
    private GameRules CreateGameRules() {
        return [ new DiagonalMoveRule(), new EnPassantRule(), new FileAndRankMoveRule(), new KnightMoveRule(), new PawnMoveRule() ];
    }
    
    public ChessGame CreateStandardGame() {
        return new ChessGame(ChessBoard.StandardGame, PieceColor.White, CreateGameRules());
    }
    
    public ChessGame CreateGameFromPosition(IChessBoard initialBoard, PieceColor playerToMove) {
        return new ChessGame(initialBoard, playerToMove, CreateGameRules());
    }
}