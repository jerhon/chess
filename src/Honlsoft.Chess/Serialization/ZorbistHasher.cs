﻿using System.Globalization;
using System.Runtime.CompilerServices;

namespace Honlsoft.Chess.Serialization;

// TODO: this could be optimized to use less memory for a few special cases, like castling, but the goal right now is just to get something basic working.

/// <summary>
/// This class is used to generate a zorbist hash for a given chess position.  This is used to detect if a position has been seen before.
/// </summary>
public class ZorbistHasher
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
    
    private const int IndexCount = 18;
    
    private static readonly ulong _initialHash = 0x3edbbeb54c9392fb;
    private static ulong[] _hashKeys;
        
    public static ulong[] ReadHashes()
    {
        List<ulong> hashes = new List<ulong>();
        using var hashesStream = typeof(ZorbistHasher).Assembly.GetManifestResourceStream("Honlsoft.Chess.Serialization.ZorbistHashes.txt");
        var reader = new StreamReader(hashesStream);
        string line = reader.ReadLine();
        while (!string.IsNullOrWhiteSpace(line)) 
        {
            hashes.Add(ulong.Parse(line, NumberStyles.HexNumber));
            line = reader.ReadLine(); 
        }

        return hashes.ToArray();
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
    public ZorbistHasher()
    {
        _hashKeys = ReadHashes();
    }


    /// <summary>
    /// Calculates the zorbist hash for a given chess position.
    /// </summary>
    /// <param name="chessPosition">The chess position to calculate.</param>
    /// <returns>The hash value.</returns>
    public ulong CalculateHash(IChessPosition chessPosition)
    {
        ulong currentHash = _initialHash;
        
        var whiteCastlingRights = chessPosition.GetCastlingRights(PieceColor.White);
        var blackCastlingRights = chessPosition.GetCastlingRights(PieceColor.Black);

        foreach (var squareName in SquareName.AllSquares())
        {
            var square = chessPosition.GetSquare(squareName);
            if (!square.HasPiece)
            {
                continue;
            }

            var castlingRights = square.Piece.Color == PieceColor.White ? whiteCastlingRights : blackCastlingRights;
            var pieceHash = GetPieceHash(square, castlingRights, chessPosition.EnPassantTarget);
            currentHash ^= pieceHash;
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
        var pieceHash = GetPieceHash(square, castlingRights, enPassantTarget);
        return hash ^ pieceHash;
    }
    
    private ulong GetPieceHash(Square square, CastlingSide[]? castlingRights, SquareName enPassantTarget)
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
        
        return _hashKeys[ (positionIndex * IndexCount) + (int)pieceIndex ];
    }
   
    const int ExtraPositionsStartIdx = 64 * IndexCount;
    
    public static ZorbistHasher Default { get; } = new ZorbistHasher();
}