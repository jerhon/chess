using System.Runtime.InteropServices;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Honlsoft.Chess.ChessDotCom.Client;

/// <summary>
/// Factory to create a ChessDotComClient with a custom contact info.
/// </summary>
public class ChessDotComClientFactory {
    
    /// <summary>
    /// Create a ChessDotComClient with contact info to reach the individual if to many API requests are made.
    /// </summary>
    /// <param name="contactInfo">The contact info of the user making the requests</param>
    /// <returns>The client.</returns>
    public ChessDotComClient CreateClient(string contactInfo) {
        
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", contactInfo);
        ChessDotComClient client = new ChessDotComClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient));
        return client;
    }
}