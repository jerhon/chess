// See https://aka.ms/new-console-template for more information



using System.CommandLine;
using Honlsoft.Chess.ChessDotCom.Console.UseCases;

RootCommand command = new RootCommand();

Command gamesCommand = new Command("games", "Commands for working with chess.com games.");
Command importCommand = new Command("import", "Import games from chess.com.");

Option<DirectoryInfo> outDirOption = new Option<DirectoryInfo>("--out-dir", "The directory to output the games to.");
outDirOption.IsRequired = true;

Option<string> userNameOption = new Option<string>("--user-name", "The username to import games for.");
userNameOption.IsRequired = true;

Option<string> yearOption = new Option<string>("--year", "The year to import games for.");
yearOption.IsRequired = true;

Option<string> monthOption = new Option<string>("--month", "The month to import games for.");
monthOption.IsRequired = true;

importCommand.AddOption(outDirOption);
importCommand.AddOption(userNameOption);
importCommand.AddOption(yearOption);
importCommand.AddOption(monthOption);

gamesCommand.AddCommand(importCommand);

command.AddCommand(gamesCommand);

importCommand.SetHandler(async (DirectoryInfo outDir, string userName, string year, string month) => {
    await new ImportGames().ImportGamesAsync(outDir, userName, year, month);
}, outDirOption, userNameOption, yearOption, monthOption);

return await command.InvokeAsync(args);