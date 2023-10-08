namespace Honlsoft.Chess; 

public interface IChessGame {
    
    public PieceColor CurrentPlayer { get; }
    
    public IChessPosition CurrentPosition { get; }
    
    public SquareName EnPassantTarget { get; }
    
    
    public ChessGameState GameState { get; }
    
}