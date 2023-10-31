using System.Reflection;

namespace Honlsoft.Chess.Serialization.Pgn.Tests.Samples; 

public class PgnSample {
    public static string Read(string sampleName) {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
        basePath = basePath.Replace("file:\\", "").Replace("file:", "");
        string actualPath = Path.Combine(basePath, "Samples", sampleName);
        
        return File.ReadAllText(actualPath);
    }
}