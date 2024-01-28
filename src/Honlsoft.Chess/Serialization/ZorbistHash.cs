using System.Runtime.CompilerServices;

namespace Honlsoft.Chess.Serialization;

// TODO: this could be optimized to use less memory for a few special cases, like castling, but the goal right now is just to get something basic working.

/// <summary>
/// This class is used to generate a zorbist hash for a given chess position.  This is used to detect if a position has been seen before.
/// </summary>
public class ZorbistHash
{
    private enum Indexes : int
    {
        WhitePawn,
        WhiteRook,
        WhiteKnight,
        WhiteBishop,
        WhiteQueen,
        WhiteKing,
        WhiteRookCastleQueenside,
        WhiteRookCastleKingside,
        WhitePawnEnPassantCapture,
        BlackPawn,
        BlackRook,
        BlackKnight,
        BlackBishop,
        BlackQueen,
        BlackKing,
        BlackRookCastleQueenside,
        BlackRookCastleKingside,
        BlackPawnEnPassantCapture,
    }
    
    private static ulong _initialHash = 0xfedbbeb54c9392fb;
    private static ulong[,]? _hashKeys = new ulong [64,18];
        
    private void GenerateKeys()
    {
        if (_hashKeys == null)
        {
            const int positionCount = 64;
            const int pieceNumber = 18;
            ulong[,] hashKeys = new ulong[64, 18];
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    hashKeys[i, j] = NextUInt64();
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong NextUInt64() {
        Span<byte> resultBytes = stackalloc byte[8];
        Random.Shared.NextBytes(resultBytes);
        return BitConverter.ToUInt64(resultBytes);
    }
    
    /// <summary>
    /// Create the zorbist hash from an initial chess position.
    /// </summary>
    public ZorbistHash()
    {
        GenerateKeys();
    }


    /// <summary>
    /// Calculates the zorbist hash for a given chess position.
    /// </summary>
    /// <param name="chessPosition">The chess position to calculate.</param>
    /// <returns>The hash value.</returns>
    public ulong Calculate(IChessPosition chessPosition)
    {
        GenerateKeys();
        
        ulong currentHash = _initialHash;
        
        var whiteCastlingRights = chessPosition.GetCastlingRights(PieceColor.White);
        var blackCastlingRights = chessPosition.GetCastlingRights(PieceColor.Black);
        
        foreach (var squareName in SquareName.AllSquares())
        {
            var square = chessPosition.GetSquare(squareName);
            var castlingRights = square.Piece.Color == PieceColor.White ? whiteCastlingRights : blackCastlingRights;
            var pieceIndex = GetPieceIndex(square, castlingRights, chessPosition.EnPassantTarget);
            currentHash ^= _hashKeys[pieceIndex.PositionIndex, pieceIndex.TypeIndex];
        }

        return currentHash;
    }
    
    /// <summary>
    /// Updates the hash with the piece.  This effectively works like a toggle if a given piece is in the hash it will remove it. Otherwise it will add it back in.
    /// </summary>
    /// <param name="square">The square with the piece to add.</param>
    /// <param name="castlingRights">The castling rights to consider.</param>
    /// <param name="enPassantTarget">The en passant target.</param>
    public ulong UpdateHash(ulong hash, Square square, CastlingSide[]? castlingRights, SquareName enPassantTarget)
    {
        var pieceIndex = GetPieceIndex(square, castlingRights, enPassantTarget);
        return hash ^ _hashKeys[pieceIndex.PositionIndex, pieceIndex.TypeIndex];
    }
    
    private (int PositionIndex, int TypeIndex) GetPieceIndex(Square square, CastlingSide[]? castlingRights, SquareName enPassantTarget)
    {
        // need to double check I have this right, but not a big deal if they get rotated.
        int positionIndex = square.Name.SquareRank.Index * 8 + square.Name.SquareFile.Index;
        
        if (square.Piece == null)
        {
            throw new ArgumentOutOfRangeException(nameof(square));
        }
        var pieceIndex =  square.Piece switch
        {
            { Type: PieceType.Pawn } => square.Piece.Color == PieceColor.White ? Indexes.WhitePawn : Indexes.BlackPawn,
            { Type: PieceType.Bishop } => square.Piece.Color == PieceColor.White ? Indexes.WhiteBishop : Indexes.BlackBishop,
            { Type: PieceType.Knight } => square.Piece.Color == PieceColor.White ? Indexes.WhiteKnight : Indexes.BlackKnight,
            { Type: PieceType.Rook } => square.Piece.Color == PieceColor.White ? Indexes.WhiteRook : Indexes.BlackRook,
            { Type: PieceType.Queen } => square.Piece.Color == PieceColor.White ? Indexes.WhiteQueen : Indexes.BlackQueen,
            { Type: PieceType.King } => square.Piece.Color == PieceColor.White ? Indexes.WhiteKing : Indexes.BlackKing, 
        };

        if (square.Piece.Type == PieceType.Rook && castlingRights != null)
        {
            if (castlingRights.Contains(CastlingSide.Kingside))
            {
                pieceIndex = square.Piece.Color == PieceColor.White ? Indexes.WhiteRookCastleKingside : Indexes.BlackRookCastleKingside;
            }
            else if (castlingRights.Contains(CastlingSide.Queenside))
            {
                pieceIndex = square.Piece.Color == PieceColor.White ? Indexes.WhiteRookCastleQueenside : Indexes.BlackRookCastleQueenside;
            }
        }
        
        if (square.Piece.Type == PieceType.Pawn && square.Name == enPassantTarget)
        {
            pieceIndex = square.Piece.Color == PieceColor.White ? Indexes.WhitePawnEnPassantCapture : Indexes.BlackPawnEnPassantCapture;
        }
        
        return (positionIndex, (int)pieceIndex);
    }
    
}