using System.CommandLine;
using Honlsoft.Chess.Console.UseCases;
using Spectre.Console;

namespace Honlsoft.Chess.Console.CommandLine;

public class CommandLineFactory(IServiceProvider serviceProvider)
{
    public RootCommand CreateCommand()
    {
        RootCommand rootCmd = new RootCommand("Chess utilities.");
        
        rootCmd.AddCommand(CreateDatabaseCommands());
        rootCmd.AddCommand(CreatePlayGameCommands());
        rootCmd.AddCommand(CreateTestCommand());
        rootCmd.AddCommand(CreateReplayCommand());
        
        return rootCmd;
    }

    public Command CreateDatabaseCommands()
    {
        Command databaseCmd = new Command("database", "Chess database utilities.");
        Command importCmd = new Command("import", "Import a PGN file into the database.");

        var databaseOption = new Option<FileInfo>("--database", "The database to import into.")
        {
            IsRequired = true
        };
        var fileOption = new Option<FileInfo?>("--file", "The PGN file to import.");
        var directoryOption = new Option<DirectoryInfo?>("--directory", "The directory containing the PGN files to import.");
            
        
        importCmd.AddOption(fileOption);
        importCmd.AddOption(databaseOption);
        importCmd.AddOption(directoryOption);
        
        importCmd.SetHandler(async (file, directory, database) =>
        {
            var importer = new ImportGames();
            
            if (file != null)
            {
                AnsiConsole.MarkupLine($"[yellow]Importing {file.Name}[/]");
                await importer.ImportGameAsync(file, database);
                return;
            } else if (directory != null) {
                foreach (var directoryFile in directory.GetFiles("*.pgn")) {
                    AnsiConsole.MarkupLine($"[yellow]Importing {directoryFile.Name}[/]");
                    await importer.ImportGameAsync(directoryFile, database);
                }
            }
            else {
                AnsiConsole.MarkupLine("[red]No file or directory specified.[/]");
                Environment.Exit(-1);
            }
        },fileOption, directoryOption, databaseOption);
        
        databaseCmd.AddCommand(importCmd);

        return databaseCmd;
    }

    public Command CreatePlayGameCommands()
    {
        Command playGameCmd = new Command("play", "Play a game of chess.");
        
        playGameCmd.SetHandler((Func<Task>)(async () =>
        {
            var gameRunner = new PlayGame(serviceProvider);
            await gameRunner.PlayAsync();
        }));

        return playGameCmd;
    }

    public Command CreateTestCommand() {
        Command testCommand = new Command("test", "Test the chess engine against a set of PGN files.");
        
        var directoryOption = new Option<DirectoryInfo>("--directory", "The directory containing the PGN files to test.")
        {
            IsRequired = true
        };
        
        testCommand.AddOption(directoryOption);
        
        testCommand.SetHandler(async (directory) =>
        {
            var tester = new Test();
            await tester.TestAsync(directory);
        }, directoryOption);
        
        return testCommand;
    }

    public Command CreateReplayCommand() {
        
        Command replayCommand = new Command("replay", "Replay a PGN file.");
        
        var pgnOption = new Option<FileInfo>("--pgn", "The PGN file to replay.")
        {
            IsRequired = true
        };
        
        replayCommand.AddOption(pgnOption);
        
        replayCommand.SetHandler(async (pgnFile) =>
        {
            var replay = new ReplayGame();
            await replay.ReplayAsync(pgnFile);
        }, pgnOption);
        
        return replayCommand;
    }
}