using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess;

public record PlayerTurn(IChessBoard AfterState, ChessMove Move, PieceColor Player);