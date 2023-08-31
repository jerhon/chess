namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciParameter {
    
    /// <summary>
    /// The key for the parameter. 
    /// </summary>
    public string? Key { get; set; }
    
    /// <summary>
    /// The value for the parameter.
    /// </summary>
    public string Value { get; set; }
}