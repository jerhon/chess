using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Honlsoft.Chess.ChessDotCom.Console.UseCases;

public class ImportGames {

    public async Task ImportGamesAsync(DirectoryInfo outDir, string userName,  string year, string month) {

        ChessDotComClient client = new ChessDotComClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider()));
        var games = await client.Player[userName].Games[year][month].GetAsync();
        foreach (var game in games.Games) {
            
            var opponent = game.White.Username == userName ? game.Black.Username : game.White.Username;
            
            var gameFile = new FileInfo(Path.Combine(outDir.FullName, $"{game.EndTime}-{opponent}.pgn"));
            await File.WriteAllTextAsync(gameFile.FullName, game.Pgn);
        }
    }
    
}