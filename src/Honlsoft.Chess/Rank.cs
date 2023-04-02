namespace Honlsoft.Chess; 

public record Rank(int Number) {
    
    public static Rank Rank1 = new(1);
    public static Rank Rank2 = new(2);
    public static Rank Rank3 = new(3);
    public static Rank Rank4 = new(4);
    public static Rank Rank5 = new(5);
    public static Rank Rank6 = new(6);
    public static Rank Rank7 = new(7);
    public static Rank Rank8 = new(8);
    
    public static Rank[] AllRanks = { Rank8, Rank7, Rank6, Rank5, Rank4, Rank3, Rank2, Rank1 };
    
    public override string ToString() {
        return Number.ToString();
    }
}