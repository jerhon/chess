using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

public class UciOptionCommand : UciCommand {
    
    
    public string Default { get; set; }
    
    public string Min { get; set; }
    
    public string Max { get; set; }
    
    public string Type { get; set; }
    
    
    public UciOptionCommand(string command, IEnumerable<string>? parameters = null) : base(command, parameters) {
    }
}