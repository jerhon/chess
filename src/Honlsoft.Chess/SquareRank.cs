namespace Honlsoft.Chess; 

/// <summary>
/// The Rank for the Chess board. This is the vertical axis of the board. It is represented by a number from 1-8.
/// </summary>
/// <param name="Number">The number of the Rank.</param>
public record SquareRank(int Number)
{
    public static readonly SquareRank Rank1 = new(1);
    public static readonly SquareRank Rank2 = new(2);
    public static readonly SquareRank Rank3 = new(3);
    public static readonly SquareRank Rank4 = new(4);
    public static readonly SquareRank Rank5 = new(5);
    public static readonly SquareRank Rank6 = new(6);
    public static readonly SquareRank Rank7 = new(7);
    public static readonly SquareRank Rank8 = new(8);
    
    /// <summary>
    /// All the Ranks.
    /// </summary>
    public static SquareRank[] AllRanks = { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
    
    /// <summary>
    /// Add to the rank. 
    /// </summary>
    /// <param name="value"></param>
    public SquareRank? Add(int value) {
        int newRank = Number + value;
        if (newRank > 8 || newRank < 1) {
            return null;
        }
        return new SquareRank(newRank);
    }

    
    /// <summary>
    /// Returns the distance between two files.
    /// </summary>
    /// <param name="other">The other rank to compare to.</param>
    /// <returns>The distance between the files.  If the rank is vertically up the board, it will be positive.  If the file is downward, it will be negative.</returns>
    public int Distance(SquareRank other) {
        return other.Number - this.Number;
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
    /// The zero based index of the rank.
    /// </summary>
    public int Index => this.Number - 1;
    
    /// <summary>
    /// Returns the number to a string.
    /// </summary>
    /// <returns>Returns the number as a string.</returns>
    public override string ToString() {
        return Number.ToString();
    }

    public IEnumerable<SquareRank> ToEnd(bool inclusive = false) {
        return ToRange(8, inclusive);
    }

    public IEnumerable<SquareRank> ToStart(bool inclusive = false) {
        return ToRange(-8, inclusive);
    }
    
    
    public IEnumerable<SquareRank> ToRange(int limit, bool inclusive = false) {
        int i = 0;
        int absLimit = Math.Abs(limit);
        SquareRank? rank = this;
        while (rank != null && i < absLimit) {
            if ((rank == this && inclusive) || (rank != this)) {
                yield return rank;
                i++;
            }
            rank = rank?.Add(Math.Sign(limit));
        }
    }

    /// <summary>
    /// Parses the chess rank.
    /// </summary>
    /// <param name="rankChar">The character for the rank</param>
    /// <returns>A rank, or null if one does not exist.</returns>
    public static SquareRank? Parse(char rankChar) {
        return Char.ToLowerInvariant(rankChar) switch {
            '1' => Rank1,
            '2' => Rank2,
            '3' => Rank3,
            '4' => Rank4,
            '5' => Rank5,
            '6' => Rank6,
            '7' => Rank7,
            '8' => Rank8,
            _ => null       
        };
    }

    /// <summary>
    /// Parses the chess rank.
    /// </summary>
    /// <param name="rankNumber">The number of the rank</param>
    /// <returns>A SquareRank object representing the rank number.</returns>
    public static SquareRank? Parse(int rankNumber) {
        return rankNumber switch {
            1 => Rank1,
            2 => Rank2,
            3 => Rank3,
            4 => Rank4,
            5 => Rank5,
            6 => Rank6,
            7 => Rank7,
            8 => Rank8,
            _ => null       
        }; 
    }
}