namespace Honlsoft.Chess; 

public interface IChessGame {
    
    public PieceColor CurrentPlayer { get; }
    
    public IChessBoard CurrentBoard { get; }
}