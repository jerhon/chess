// See https://aka.ms/new-console-template for more information



using System.CommandLine;
using Honlsoft.Chess.ChessDotCom.Client;
using Honlsoft.Chess.ChessDotCom.Console.UseCases;

RootCommand command = new RootCommand();


Command gamesCommand = new Command("games", "Commands for working with chess.com games.");
Command exportGames = new Command("export", "export games from chess.com.");

Option<string> contactInfoOpt = new Option<string>("--contact-info", "The contact information to use from chess.com for the request.");
contactInfoOpt.IsRequired = true;

Option<DirectoryInfo> outDirOption = new Option<DirectoryInfo>("--out-dir", "The directory to output the games to.");
outDirOption.IsRequired = true;

Option<string> userNameOption = new Option<string>("--user-name", "The username to import games for.");
userNameOption.IsRequired = true;

Option<string> yearOption = new Option<string>("--year", "The year to import games for.");
yearOption.IsRequired = true;

Option<string> monthOption = new Option<string>("--month", "The month to import games for.");
monthOption.IsRequired = true;

exportGames.AddOption(outDirOption);
exportGames.AddOption(userNameOption);
exportGames.AddOption(yearOption);
exportGames.AddOption(monthOption);
exportGames.AddOption(contactInfoOpt);

gamesCommand.AddCommand(exportGames);

command.AddCommand(gamesCommand);

exportGames.SetHandler(async (string contactInfo, DirectoryInfo outDir, string userName, string year, string month) => {
    await new ExportGames(new ChessDotComClientFactory()).ImportGamesAsync(outDir, contactInfo, userName, year, month);
}, contactInfoOpt, outDirOption, userNameOption, yearOption, monthOption);

return await command.InvokeAsync(args);