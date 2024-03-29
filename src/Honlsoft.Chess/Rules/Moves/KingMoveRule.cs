﻿namespace Honlsoft.Chess.Rules; 

public class KingMoveRule : IMoveRule {

    public bool IsApplicable(IChessPosition chessPosition, SquareName from) {
        return chessPosition.GetSquare(from) is { Piece: { Type: PieceType.King } };
    }
    
    public IChessMove[] GetCandidateMoves(IChessPosition chessPosition, SquareName from) {

        if (!IsApplicable(chessPosition, from)) {
            return [];
        }

        List<IChessMove> moves = new();
        
        // pretty simple, just return any empty squares in the immediate vicinity of the king
        // we don't consider threats as that will be dealt with later and it would create some nasty circular references
        for (int fileOffset = -1; fileOffset <= 1; fileOffset++) {
            for (int rankOffset = -1; rankOffset <= 1; rankOffset++) {

                if (fileOffset == 0 && rankOffset == 0) {
                    continue;
                }
                
                var toSquareFile = from.SquareFile.Add(fileOffset);
                var toSquareRank = from.SquareRank.Add(rankOffset);

                if (toSquareFile == null || toSquareRank == null) {
                    continue;
                }

                var fromSquare = chessPosition.GetSquare(from);
                var toSquare = chessPosition.GetSquare(new SquareName(toSquareFile, toSquareRank));

                if (toSquare.HasPiece && fromSquare.Piece.Color == toSquare.Piece.Color) {
                    continue;
                }

                moves.Add( new SimpleMove(from, toSquare.Name));
            }
        }

        Square square = chessPosition.GetSquare(from);
        PieceColor color = square.Piece.Color;
        if (from.SquareFile == SquareFile.e) {
            if (from.SquareRank == SquareRank.Rank1 || from.SquareRank == SquareRank.Rank8) {
                if (chessPosition.CanCastle(color, CastlingSide.Queenside)) {
                    moves.Add(new CastlingMoveRule(color, CastlingSide.Queenside));
                }
                if (chessPosition.CanCastle(color, CastlingSide.Queenside)) {
                    moves.Add(new CastlingMoveRule(color, CastlingSide.Kingside));
                }
            }    
        }
        
        return moves.ToArray();
    }
    
}