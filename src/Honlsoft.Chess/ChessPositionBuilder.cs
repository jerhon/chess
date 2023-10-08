using Honlsoft.Chess.Rules;

namespace Honlsoft.Chess; 

/// <summary>
/// Builds a chess board with pieces on it.
/// </summary>
public class ChessPositionBuilder : IChessPosition {

    private readonly Dictionary<SquareName, Square> _squares = new();
    private readonly HashSet<(PieceColor, CastlingSide)> _castlingRights = [];
    
    public SquareName? EnPassantTarget { get; private set; }
    public bool CanCastle(PieceColor playerColor, CastlingSide castleSide) {

        return _castlingRights.Contains((playerColor, castleSide));
    }

    public ChessPositionBuilder WithSquare(Square square) {
        _squares[square.Name] = square;
        return this;
    } 

    public ChessPositionBuilder WithSquare(SquareName squareName, PieceType type, PieceColor color) {
        var square = new Square(squareName, new Piece(type, color));
        return WithSquare(square);
    }

    public ChessPositionBuilder WithSquare(string pieceAndSquareNotation) {
        var square = Square.Parse(pieceAndSquareNotation);
        return WithSquare(square);
    }

    public ChessPositionBuilder FromBoard(IChessPosition chessPosition) {
        this.EnPassantTarget = chessPosition.EnPassantTarget;
        foreach (var squareName in SquareName.AllSquares()) {
            var square = chessPosition.GetSquare(squareName);
            if (square.HasPiece) {
                WithSquare(square);
            }
        }
        return this;
    }

    /// <summary>
    /// Moves the piece on one square to another.  Does not take any other rules into account.
    /// </summary>
    /// <param name="from">from</param>
    /// <param name="to">to</param>
    /// <returns></returns>
    public ChessPositionBuilder Move(SquareName from, SquareName to) {
        // Move the piece off the square... there is nothing there anymore
        if (_squares.ContainsKey(from)) {
            var fromSquare = _squares[from];
            var toSquare = new Square(to, fromSquare.Piece);
            _squares.Remove(from);
            _squares[to] = toSquare;
        }
        
        return this;
    }

    public ChessPositionBuilder Move(IChessMove move) {
        move.ApplyMove(this);

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

    public ChessPositionBuilder WithCastlingRights(PieceColor pieceColor, CastlingSide side, bool canCastle) {

        if (canCastle) {
            _castlingRights.Add((pieceColor, side));
        } else {
            _castlingRights.Remove((pieceColor, side));
        }

        return this;
    }
    
    public IChessPosition Build() {
        HashSet<(PieceColor, CastlingSide)> castling = new();
        
        var chessPositionBuilder = new ChessPositionBuilder() {
            EnPassantTarget = EnPassantTarget,
        };
        
        foreach (var castle in _castlingRights) {
            chessPositionBuilder.WithCastlingRights(castle.Item1, castle.Item2, true);
        }

        foreach (var square in _squares) {
            chessPositionBuilder.WithSquare(square.Value);
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
                WithSquare(position, piece.Value, color.Value);
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


    public static IChessPosition StandardGame = new ChessPositionBuilder().AddStandardGamePieces().AddAllCastling().Build();
}