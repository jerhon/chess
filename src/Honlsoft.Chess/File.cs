namespace Honlsoft.Chess; 

/// <summary>
/// The chess file.
/// </summary>
/// <param name="Name">the name of the file</param>
public record File(char Name) {
    
    public static File a = new File('a');
    public static File b = new File('b');
    public static File c = new File('c');
    public static File d = new File('d');
    public static File e = new File('e');
    public static File f = new File('f');
    public static File g = new File('g');
    public static File h = new File('h');
    
    public static File[] AllFiles = { a,b,c,d,e,f,g,h };
    
    /// <summary>
    /// Adds a number to a file.
    /// </summary>
    /// <param name="number">The number of spaces to add.</param>
    /// <returns>The new file</returns>
    /// <exception cref="InvalidOperationException">An invalid operation is thrown.</exception>
    public File? Add(int number) {
        char name = (char)(Name + number);
        if (name > 'h' || name < 'a') {
            return null;
        }
        return new File(name);
    }
    
    /// <summary>
    /// This is the first File.
    /// </summary>
    public bool IsFirst => this.Name == File.a.Name;

    /// <summary>
    /// This is the last file.
    /// </summary>
    public bool IsLast => this.Name == File.g.Name;
    
    /// <summary>
    /// Returns the name of the file.
    /// </summary>
    /// <returns>The name of the file to use.</returns>
    public override string ToString() {
        return Name.ToString();
    }
}