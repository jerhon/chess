
namespace Honlsoft.Chess.Wasm;


public partial class MainJS
{

    public static async Task Main()
    {
        if (!OperatingSystem.IsBrowser())
        {
            throw new PlatformNotSupportedException("This demo is expected to run on browser platform");
        }
        
        Console.WriteLine("Hello, world!");
    }
}