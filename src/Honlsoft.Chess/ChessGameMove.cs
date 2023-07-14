namespace Honlsoft.Chess;

public record ChessGameMove(ChessGameState InitialState, ChessGameState FinalState, SquareName FromSquare, SquareName ToSquare);