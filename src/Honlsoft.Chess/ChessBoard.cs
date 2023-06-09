﻿using System.Collections;
using System.Drawing;

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
    /// Transitions the game state by moving a piece from one position to another.
    /// This does not validate or ensure the moves are valid.
    /// It only creates the new board in the new state.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>The chess board with the new piece positions.</returns>
    public ChessBoard Move(SquareName from, SquareName to) {
        var fromSquare = GetSquare(from);
        var toSquare = GetSquare(to);
        var newBoard = Clone();
        var newFromSpace = new Square(from, null);
        var newToSpace = new Square(to, fromSquare.Piece);

        newBoard.SetSquare(newFromSpace);
        newBoard.SetSquare(newToSpace);
        return newBoard;
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

    
    /// <summary>
    /// Sets a piece at a particular position on the board.
    /// </summary>
    /// <param name="squareName">The name of the square.</param>
    /// <param name="piece">The piece to place on the square.</param>
    private ChessBoard SetPiece(SquareName squareName, Piece piece) {
        var chessBoard = this.Clone();
        chessBoard.SetSquare(new Square(squareName, piece));
        return chessBoard;
    }

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
    /// Creates a new game with the chess pieces in their standard positions.
    /// </summary>
    /// <returns></returns>
    public static ChessBoard NewStandardGame() {
        var gameState = new ChessBoard();
        foreach (var position in gameState) {
            var color = GetInitialColor(position.Name);
            var piece = GetPieceType(position.Name);

            if (piece != null && color != null) {
                gameState.SetSquare(new Square(position.Name, new Piece(piece.Value, color.Value)));
            }
        }
        return gameState;
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
    

    private static PieceType? GetPieceType(SquareName position) =>
        (position.File.Name, position.Rank.Number) switch {
            ('a' or 'h', 1 or 8) => PieceType.Rook,
            ('b' or 'g', 1 or 8) => PieceType.Knight,
            ('c' or 'f', 1 or 8) => PieceType.Bishop,
            ('d', 1 or 8) => PieceType.Queen,
            ('e', 1 or 8) => PieceType.King,
            (_, 7 or 2) => PieceType.Pawn,
            _ => null
        };

    private static PieceColor? GetInitialColor(SquareName position) =>
        position.Rank.Number switch {
            1 or 2 => PieceColor.White,
            7 or 8 => PieceColor.Black,
            _ => null
        };

    private ChessBoard Clone() {
        var newBoard = new ChessBoard();
        foreach (var boardSpace in this) {
            newBoard.SetSquare(boardSpace);
        }
        return newBoard;
    }

    /// <summary>
    /// Returns a chessboard set up for a classical game of chess.
    /// </summary>
    public static ChessBoard StandardGame = NewStandardGame();
}