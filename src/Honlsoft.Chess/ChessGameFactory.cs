using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

public class ChessGameFactory {

    public MoveRules CreateMoveRules() {
        return new MoveRules([ new DiagonalMoveRule(), new EnPassantRule(), new FileAndRankMoveRule(), new KnightMoveRule(), new PawnMoveRule(), new KingMoveRule() ]);
    }
    
    public GameRules CreateGameRules() {
        return new GameRules(CreateMoveRules());
    }
    
    public ChessGame CreateStandardGame() {
        return new ChessGame(ChessPositionBuilder.StandardGame, CreateGameRules());
    }
    
    public ChessGame CreateGameFromPosition(IChessPosition initialPosition) {
        return new ChessGame(initialPosition, CreateGameRules());
    }
}