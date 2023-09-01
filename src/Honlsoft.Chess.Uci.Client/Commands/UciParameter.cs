namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciParameter {

    public UciParameter(string? key, string? value) {
        Key = key;
        Value = value;
    }
    
    /// <summary>
    /// The key for the parameter. 
    /// </summary>
    public string? Key { get; set; }
    
    /// <summary>
    /// The value for the parameter.
    /// </summary>
    public string? Value { get; set; }
}