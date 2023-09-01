namespace Honlsoft.Chess.Uci.Client.Commands; 

/// <summary>
/// Facade over a UCI option command.
/// </summary>
public class UciOption {

    public UciOption(UciCommand command) {
        foreach (var parameter in command.Parameters) {
            if (parameter.Key == "max") {
                Max = parameter.Value;
            } else if (parameter.Key == "min") {
                Min = parameter.Value;
            } else if (parameter.Key == "value") {
                Value = parameter.Value;
            } else if (parameter.Key == "default") {
                Default = parameter.Value;
            } else if (parameter.Key == "type") {
                Type = parameter.Value;
            } else if (parameter.Key == "name") {
                Name = parameter.Value;
            }
        }
    }
    
    
    public string? Max { get; set; }
    
    public string? Min { get; set; }
    
    public string? Value { get; set; }
    
    public string? Default { get; set; }
    
    public string Type { get; set; }
    
    public string Name { get; set; }
}