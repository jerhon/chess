namespace Honlsoft.Chess; 

/// <summary>
/// The chess file.
/// </summary>
/// <param name="Name">the name of the file</param>
public record SquareFile(char Name) {
    
    public static SquareFile a = new SquareFile('a');
    public static SquareFile b = new SquareFile('b');
    public static SquareFile c = new SquareFile('c');
    public static SquareFile d = new SquareFile('d');
    public static SquareFile e = new SquareFile('e');
    public static SquareFile f = new SquareFile('f');
    public static SquareFile g = new SquareFile('g');
    public static SquareFile h = new SquareFile('h');
    
    public static SquareFile[] AllFiles = { a,b,c,d,e,f,g,h };
    
    /// <summary>
    /// Adds a number to a file.
    /// </summary>
    /// <param name="number">The number of spaces to add.</param>
    /// <returns>The new file</returns>
    /// <exception cref="InvalidOperationException">An invalid operation is thrown.</exception>
    public SquareFile? Add(int number) {
        char name = (char)(Name + number);
        if (name > 'h' || name < 'a') {
            return null;
        }
        return new SquareFile(name);
    }
    
    /// <summary>
    /// This is the first File.
    /// </summary>
    public bool IsFirst => this.Name == SquareFile.a.Name;

    /// <summary>
    /// This is the last file.
    /// </summary>
    public bool IsLast => this.Name == SquareFile.g.Name;
    
    /// <summary>
    /// Returns the name of the file.
    /// </summary>
    /// <returns>The name of the file to use.</returns>
    public override string ToString() {
        return Name.ToString();
    }

    public IEnumerable<SquareFile> ToEnd(bool inclusive = false) {
        return ToRange(8, inclusive);
    }


    public IEnumerable<SquareFile> ToStart(bool inclusive = false) {
        return ToRange(-8, inclusive);
    }

    public IEnumerable<SquareFile> ToRange(int limit, bool inclusive = false) {
        int i = 0;
        SquareFile? file = this;
        int step = Math.Sign(limit);
        int absLimit = Math.Abs(limit);
        while (file != null && i < absLimit) {
            if ((file == this && inclusive) || file != this) {
                yield return file;
                i++;
            }
            file = file?.Add(step);
        }
    }

    public static SquareFile? Parse(char fileChar) {
        return fileChar switch {
            'a' => Chess.SquareFile.a,
            'b' => Chess.SquareFile.b,
            'c' => Chess.SquareFile.c,
            'd' => Chess.SquareFile.d,
            'e' => Chess.SquareFile.e,
            'f' => Chess.SquareFile.f,
            'g' => Chess.SquareFile.g,
            'h' => Chess.SquareFile.h,
            _ => throw new FormatException($"{fileChar} not a valid file.")
        };
    }

    public int Index => Name - 'a';
}