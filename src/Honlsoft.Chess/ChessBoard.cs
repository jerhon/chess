using System.Collections;

namespace Honlsoft.Chess;

/// <summary>
/// Provides the state of the chess game at the current point.
/// </summary>
public class ChessBoard : IEnumerable<Square>, IChessBoard {

    private static readonly int MaxRanks = File.AllFiles.Length;
    private static readonly int MaxFiles = Rank.AllRanks.Length;
    private readonly Square[,] _spaces = new Square[MaxFiles, MaxRanks];

    /// <summary>
    /// Creates an empty chess board with no pieces.
    /// </summary>
    public ChessBoard() {
        
        // TODO: could optimize by empty squares for all positions.
        foreach (var file in File.AllFiles) {
            foreach (var rank in Rank.AllRanks) {
                var squareName = new SquareName(file, rank);
                var square = new Square(squareName, null);
                SetSquare(square);
            }
        }
    }

    /// <summary>
    /// Creates a chess board with the given squares.  All others will be empty.
    /// </summary>
    /// <param name="squares">The squares to set on the chess board.</param>
    public ChessBoard(IEnumerable<Square> squares) {
        
        foreach (var square in squares) {
            SetSquare(square);
        }
        
        // TODO: could optimize by empty squares for all positions.
        foreach (var file in File.AllFiles) {
            foreach (var rank in Rank.AllRanks) {
                var squareName = new SquareName(file, rank);
                var fileIndex = GetFileIndex(file);
                var rankIndex = GetRankIndex(rank);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (_spaces[fileIndex, rankIndex] == null) {
                    var square = new Square(squareName, null);
                    SetSquare(square);
                }
            }
        }

    }


    /// <summary>
    /// Returns a square for name of the square.
    /// </summary>
    /// <param name="squareName">The name of the square to return.</param>
    /// <returns>The current square.</returns>
    public Square GetSquare(SquareName squareName) {
        var (file, rank) = GetIndex(squareName);
        return this._spaces[file, rank];
    }
    
    public SquareName? EnPassantTarget { get; set; }
    
    /// <summary>
    /// Enumerates all squares on the chess board.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Square> GetEnumerator() {
        for (int rank = 0; rank < MaxRanks; rank++) {
            for (int file = 0; file < MaxFiles; file++) {
                yield return _spaces[file, rank];
            }
        }
    }
    


    /// <summary>
    /// Removes a piece from the board.
    /// </summary>
    /// <param name="squareName">The square to remove the piece from.</param>
    /// <returns>A new chess board with the piece removed.</returns>
    public ChessBoard RemovePiece(SquareName squareName) {
        var chessBoard = new ChessBoard(this.Where((s) => s.Name != squareName));
        return chessBoard;
    }

    
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }


    private void SetSquare(Square space) {
        var (file, rank) = GetIndex(space.Name);
        
        this._spaces[file, rank] = space;
    }

    private int GetRankIndex(Rank rank) => rank.Number - 1;

    private int GetFileIndex(File file) => file.Name - 'a';

    
    private (int, int) GetIndex(SquareName squareName) {
        return (GetFileIndex(squareName.File), GetRankIndex(squareName.Rank));
    }
    

    /// <summary>
    /// Returns a chessboard set up for a classical game of chess.
    /// </summary>
    public static readonly IChessBoard StandardGame = new ChessBoardBuilder().AddStandardGamePieces().Build();
}