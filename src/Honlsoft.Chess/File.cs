namespace Honlsoft.Chess; 

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

    public override string ToString() {
        return Name.ToString();
    }

}