using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Honlsoft.Chess; 

public record SquareName(SquareFile SquareFile, SquareRank SquareRank) : IParsable<SquareName?> {
    
    public override string ToString() {
        return $"{SquareFile}{SquareRank}";
    }

    public static SquareName Parse(string s, IFormatProvider? provider = null) {
        if (TryParse(s, null, out var squareName)) {
            return squareName!;
        } else {
            throw new FormatException("Must be a algebraic chess number for example: a1, g8, etc...");
        }
    }
    
    public static bool TryParse(string? s, IFormatProvider? provider, [NotNullWhen(true)]out SquareName? result) {
        
        result = default;
        
        if (string.IsNullOrWhiteSpace(s)) {
            return false;
        }

        if (s.Length != 2) {
            return false;
        }

        char fileChar = Char.ToLower(s[0]);
        char rankChar = s[1];
        
        if (fileChar is < 'a' or > 'h') {
            return false;
        }

        if (rankChar is < '1' or > '8') {
            return false;
        }

        SquareFile squareFile = SquareFile.Parse(fileChar);
        

        if (!int.TryParse(rankChar.ToString(), out int rank)) {
            return false;
        }

        result = new SquareName(new SquareFile(fileChar), new SquareRank(rank));
        return true;
    }


    public SquareName? Add(int file, int rank) {
        var newFile = SquareFile.Add(file);
        var newRank = SquareRank.Add(rank);

        if (newFile == null || newRank == null) {
            return null;
        }

        return new SquareName(newFile, newRank);
    }

    public static IEnumerable<SquareName> AllSquares() {
        return Chess.SquareRank.AllRanks.SelectMany((rank) => Chess.SquareFile.AllFiles.Select((file) => new SquareName(file, rank)));
    }

    public SquareColor Color => SquareRank.Index % 2 == SquareFile.Index % 2 ? SquareColor.Dark : SquareColor.Light;
}