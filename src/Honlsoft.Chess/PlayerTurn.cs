using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess;

public record PlayerTurn(IChessPosition AfterState, IChessMove Move, PieceColor Player);