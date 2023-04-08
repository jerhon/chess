namespace Honlsoft.Chess; 

/// <summary>
/// The Rank for the Chess board.
/// </summary>
/// <param name="Number">The number of the Rank.</param>
public record Rank(int Number)
{
    public static readonly Rank Rank1 = new(1);
    public static readonly Rank Rank2 = new(2);
    public static readonly Rank Rank3 = new(3);
    public static readonly Rank Rank4 = new(4);
    public static readonly Rank Rank5 = new(5);
    public static readonly Rank Rank6 = new(6);
    public static readonly Rank Rank7 = new(7);
    public static readonly Rank Rank8 = new(8);
    
    /// <summary>
    /// All the Ranks.
    /// </summary>
    public static Rank[] AllRanks = { Rank8, Rank7, Rank6, Rank5, Rank4, Rank3, Rank2, Rank1 };
    
    /// <summary>
    /// Add to the rank. 
    /// </summary>
    /// <param name="value"></param>
    public Rank? Add(int value) {
        int newRank = Number + value;
        if (newRank > 8 || newRank < 1) {
            return null;
        }
        return new Rank(newRank);
    }


    /// <summary>
    /// This the first file.
    /// </summary>
    public bool IsFirst => this.Number == 1;

    /// <summary>
    /// This is the last file.
    /// </summary>
    public bool IsLast => this.Number == 8;
    
    /// <summary>
    /// Returns the number to a string.
    /// </summary>
    /// <returns>Returns the number as a string.</returns>
    public override string ToString() {
        return Number.ToString();
    }
}