namespace Honlsoft.Chess; 

public record SquareName(File File, Rank Rank) {
    
    public override string ToString() {
        return $"{File}{Rank}";
    }

    public static bool TryParse(string value, out SquareName? boardPosition) {

        boardPosition = default;
        
        if (string.IsNullOrWhiteSpace(value)) {
            return false;
        }

        if (value.Length != 2) {
            return false;
        }

        char fileChar = Char.ToLower(value[0]);
        char rankChar = value[1];
        
        if (fileChar is < 'a' or > 'h') {
            return false;
        }

        if (rankChar is < '1' or > '9') {
            return false;
        }

        if (!int.TryParse(rankChar.ToString(), out int rank)) {
            return false;
        }

        boardPosition = new SquareName(new File(fileChar), new Rank(rank));
        return true;
    }
    
}