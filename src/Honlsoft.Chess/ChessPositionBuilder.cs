using System.Reflection.Metadata;
using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Builds a chess board with pieces on it, this doesn't perform complex rules such as updating the counters, etc.  That is up to the chess game logic.  This sets up a board and allows altering it in ways that would even be incorrect.
/// </summary>
public class ChessPositionBuilder : IChessPosition {

    private readonly Dictionary<SquareName, Square> _squares = new();
    private readonly HashSet<(PieceColor Color, CastlingSide Side)> _castlingRights = [];
    
    public SquareName? EnPassantTarget { get; private set; }
    
    public int FullMoves { get; private set; }
    
    public int HalfMoves { get; private set; }
    
    public PieceColor PlayerToMove { get; private set; }

    public CastlingSide[] GetCastlingRights(PieceColor playerColor)
    {
        return _castlingRights.Where(x => x.Color == playerColor).Select(x => x.Side).ToArray();
    }

    public bool CanCastle(PieceColor playerColor, CastlingSide castleSide) {

        return _castlingRights.Contains((playerColor, castleSide));
    }

    public ChessPositionBuilder SetSquare(Square square) {
        _squares[square.Name] = square;
        return this;
    } 
    
    
    public ChessPositionBuilder SetSquare(SquareName squareName, Piece? piece) {
        var square = new Square(squareName, piece);
        return SetSquare(square);
    }
    
    public ChessPositionBuilder SetSquare(SquareName squareName, PieceType type, PieceColor color) {
        var square = new Square(squareName, new Piece(type, color));
        return SetSquare(square);
    }

    public ChessPositionBuilder SetSquare(string pieceAndSquareNotation) {
        var square = Square.Parse(pieceAndSquareNotation);
        return SetSquare(square);
    }

    public ChessPositionBuilder FromPosition(IChessPosition chessPosition) {
        this.EnPassantTarget = chessPosition.EnPassantTarget;
        this.FullMoves = chessPosition.FullMoves;
        this.HalfMoves = chessPosition.HalfMoves;

        var whiteCastling = chessPosition.GetCastlingRights(PieceColor.White);
        foreach (var castlingSide in whiteCastling)
        {
            this.WithCastlingRights(PieceColor.White, castlingSide, true);
        }
        var blackCastling = chessPosition.GetCastlingRights(PieceColor.Black);
        foreach (var castlingSide in blackCastling)
        {
            this.WithCastlingRights(PieceColor.Black, castlingSide, true);
        }
        
        foreach (var squareName in SquareName.AllSquares()) {
            var square = chessPosition.GetSquare(squareName);
            if (square.HasPiece) {
                SetSquare(square);
            }
        }
        return this;
    }


    public void ResetHalfMoves() {
        HalfMoves = 0;
    }

    public void IncrementFullMoves() {
        FullMoves++;
    }

    public void IncrementHalfMoves() {
        HalfMoves++;
    }

    public ChessPositionBuilder WithFullMoves(int number) {
        FullMoves = number;
        return this;
    }

    public ChessPositionBuilder WithHalfMoves(int number) {
        HalfMoves = number;
        return this;
    }

    public ChessPositionBuilder RemovePiece(SquareName squareName) {
        if (this._squares.ContainsKey(squareName)) {
            this._squares.Remove(squareName);
        }
        return this;
    }

    public ChessPositionBuilder WithEnPassantTarget(SquareName? squareName) {
        EnPassantTarget = squareName;
        return this;
    }

    public ChessPositionBuilder WithPlayerToMove(PieceColor color) {
        PlayerToMove = color;
        return this;
    }

    public ChessPositionBuilder RemoveCastlingRights(CastlingSide side) {
        _castlingRights.Remove((PlayerToMove, side));
        return this;
    }

    public ChessPositionBuilder WithCastlingRights(PieceColor pieceColor, CastlingSide side, bool canCastle) {

        if (canCastle) {
            _castlingRights.Add((pieceColor, side));
        } else {
            _castlingRights.Remove((pieceColor, side));
        }

        return this;
    }

    /// <summary>
    /// Moves a piece on a board, doesn't update any of the intermediate state for it.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public ChessPositionBuilder Move(SquareName from, SquareName to) {

        var fromSquare = GetSquare(from);
        var toSquare = GetSquare(to);

        this.RemovePiece(from);
        this.SetSquare(to, fromSquare?.Piece);
        
        return this;
    }
    
    public IChessPosition Build() {
        
        var chessPositionBuilder = new ChessPositionBuilder() {
            EnPassantTarget = EnPassantTarget,
            HalfMoves = HalfMoves,
            FullMoves = FullMoves,
            PlayerToMove = PlayerToMove,
        };
        
        foreach (var castle in _castlingRights) {
            chessPositionBuilder.WithCastlingRights(castle.Item1, castle.Item2, true);
        }

        foreach (var square in _squares) {
            chessPositionBuilder.SetSquare(square.Value);
        }

        return chessPositionBuilder;
    }
    
    public Square GetSquare(SquareName squareName) {
        if (_squares.TryGetValue(squareName, out var square)) {
            return square;
        }
        return new Square(squareName, null);
    }
    
    
    /// <summary>
    /// Creates a new game with the chess pieces in their standard positions.
    /// </summary>
    /// <returns></returns>
    public ChessPositionBuilder AddStandardGamePieces() {
        foreach (var position in SquareName.AllSquares()) {
            var color = GetInitialColor(position);
            var piece = GetInitialPieceType(position);

            if (piece != null && color != null) {
                SetSquare(position, piece.Value, color.Value);
            }
        }
        return this;
    }



    public ChessPositionBuilder AddAllCastling() {
        WithCastlingRights(PieceColor.Black, CastlingSide.Kingside, true);
        WithCastlingRights(PieceColor.Black, CastlingSide.Queenside, true);
        WithCastlingRights(PieceColor.White, CastlingSide.Kingside, true);
        WithCastlingRights(PieceColor.White, CastlingSide.Queenside, true);

        return this;
    }
    
    private static PieceType? GetInitialPieceType(SquareName position) =>
        (position.SquareFile.Name, position.SquareRank.Number) switch {
            ('a' or 'h', 1 or 8) => PieceType.Rook,
            ('b' or 'g', 1 or 8) => PieceType.Knight,
            ('c' or 'f', 1 or 8) => PieceType.Bishop,
            ('d', 1 or 8) => PieceType.Queen,
            ('e', 1 or 8) => PieceType.King,
            (_, 7 or 2) => PieceType.Pawn,
            _ => null
        };

    private static PieceColor? GetInitialColor(SquareName position) =>
        position.SquareRank.Number switch {
            1 or 2 => PieceColor.White,
            7 or 8 => PieceColor.Black,
            _ => null
        };

    


    public bool IsPawn(SquareName squareName) {
        return GetSquare(squareName) is { Piece: { Type: PieceType.Pawn } };
    }
    
    public bool IsCapture(SquareName to) {
        var square = GetSquare(to) is { HasPiece: true };
        return square;
    }

    public bool IsQueensideRook(SquareName from) {
        var piece = GetSquare(from);
        if (piece.Piece?.Type != PieceType.Rook) {
            return false;
        }

        if (from.SquareFile != SquareFile.a) {
            return false;
        }

        return from.SquareRank == SquareRank.Rank1 || from.SquareRank == SquareRank.Rank8;
    }
    
    public bool IsKingsideRook(SquareName from) {
        var piece = GetSquare(from);
        if (piece.Piece?.Type != PieceType.Rook) {
            return false;
        }

        if (from.SquareFile != SquareFile.h) {
            return false;
        }

        return from.SquareRank == SquareRank.Rank1 || from.SquareRank == SquareRank.Rank8;
    }

    public bool IsKing(SquareName from) {
        var piece = GetSquare(from);
        return PieceType.King == piece.Piece?.Type;
    }


    public void SwitchColor() {
        if (PlayerToMove == PieceColor.Black) {
            PlayerToMove = PieceColor.White;
        } else {
            PlayerToMove = PieceColor.Black;
        }
    }

    public static IChessPosition StandardGame = new ChessPositionBuilder().AddStandardGamePieces().AddAllCastling().Build();
}