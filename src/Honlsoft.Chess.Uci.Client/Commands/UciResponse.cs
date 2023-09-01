namespace Honlsoft.Chess.Uci.Client.Commands; 

public class UciResponse {


    private Dictionary<string, string> Id { get; set; }
    
    public List<UciOption> Options { get; set; }
}