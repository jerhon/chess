﻿using System.Text;

namespace Honlsoft.Chess.Serialization; 

public class FenSerializer {
    
    public string Serialize(ChessBoard chessBoard) {
        StringBuilder builder = new StringBuilder();
        foreach (var rank in Rank.Rank1.ToEnd(true).Reverse()) {
            int emptySpaces = 0;
            foreach (var file in File.a.ToEnd(true)) {
                var square = chessBoard.GetSquare(new SquareName(file, rank));
                if (square.Piece != null) {
                    if (emptySpaces > 0) {
                        builder.Append(emptySpaces.ToString());
                        emptySpaces = 0;
                    }
                    builder.Append(square.Piece.ToString());
                } else {
                    emptySpaces++;
                }
            }
            if (emptySpaces > 0) {
                builder.Append(emptySpaces.ToString());
            }
            builder.Append("/");
        }
        builder.Length -= 1;
        return builder.ToString();
    }
}