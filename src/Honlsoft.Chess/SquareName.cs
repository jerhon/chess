namespace Honlsoft.Chess; 

public record SquareName(File File, Rank Rank) : IParsable<SquareName> {
    
    public override string ToString() {
        return $"{File}{Rank}";
    }

    public static SquareName Parse(string s, IFormatProvider? provider = null) {
        if (TryParse(s, null, out var squareName)) {
            return squareName;
        } else {
            throw new FormatException("Must be a algebraic chess number for example: a1, g8, etc...");
        }
    }
    
    public static bool TryParse(string? s, IFormatProvider? provider, out SquareName result) {
        
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

        if (rankChar is < '1' or > '9') {
            return false;
        }

        if (!int.TryParse(rankChar.ToString(), out int rank)) {
            return false;
        }

        result = new SquareName(new File(fileChar), new Rank(rank));
        return true;
    }
}