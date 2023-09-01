using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

public class ChessGameFactory {


    public ChessGameFactory() {
        
    }

    public ChessGame CreateGame(IChessBoard initialBoard) {
        return new ChessGame(initialBoard,
            new GameRules(new IMoveRule[]
                { new DiagonalMoveRule(), new EnPassantRule(), new FileAndRankMoveRule(), new KnightMoveRule(), new PawnMoveRule() }));
    }
}