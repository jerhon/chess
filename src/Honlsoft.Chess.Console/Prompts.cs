﻿using Spectre.Console;

namespace Honlsoft.Chess.Console; 

public static class Prompts {


    public static SquareName AskPickAPiece(this IAnsiConsole console) {
        string prompt = "Choose a piece to move (b4, b5, etc...)";

        
        var rawSquare = AnsiConsole.Ask<string>(prompt);
        while (!SquareName.TryParse(rawSquare, null, out _)) {
            AnsiConsole.Ask<string>(prompt);
        }

        return SquareName.Parse(rawSquare);
    }

    public static (SquareName Square, PieceType? PromotionPiece) AskMoveTo(this IAnsiConsole console) {
        string prompt = "Choose a square to move to (b4, b5, a8q etc...)";
        var rawSquare = AnsiConsole.Ask<string>(prompt);
        SquareName? squareName;
        while (!SquareName.TryParse(rawSquare.Substring(0, 2), null, out squareName)) {
            rawSquare = AnsiConsole.Ask<string>(prompt);
        }

        PieceType? promotionPiece = null;
        if (rawSquare.Length > 2) {
            promotionPiece = Piece.ParsePieceType(rawSquare[2].ToString());
        }

        return (squareName, promotionPiece);
    }
    
}