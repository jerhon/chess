using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Honlsoft.Chess; 

public record SquareName(File File, Rank Rank) : IParsable<SquareName?> {
    
    public override string ToString() {
        return $"{File}{Rank}";
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

        if (!int.TryParse(rankChar.ToString(), out int rank)) {
            return false;
        }

        result = new SquareName(new File(fileChar), new Rank(rank));
        return true;
    }


    public SquareName? Add(int file, int rank) {
        var newFile = File.Add(file);
        var newRank = Rank.Add(rank);

        if (newFile == null || newRank == null) {
            return null;
        }

        return new SquareName(newFile, newRank);
    }

    public static IEnumerable<SquareName> AllSquares() {
        return Chess.Rank.AllRanks.SelectMany((rank) => Chess.File.AllFiles.Select((file) => new SquareName(file, rank)));
    }

}