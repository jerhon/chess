﻿using System.CommandLine;
using Honlsoft.Chess.Console.UseCases;

namespace Honlsoft.Chess.Console.CommandLine;

public class CommandLineFactory(IServiceProvider serviceProvider)
{
    public RootCommand CreateCommand()
    {
        RootCommand rootCmd = new RootCommand("Chess utilities.");
        
        rootCmd.AddCommand(CreateDatabaseCommands());
        rootCmd.AddCommand(CreatePlayGameCommands());
        
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
        var fileOption = new Option<FileInfo>("--file", "The PGN file to import.")
        {
            IsRequired = true
        };
        
        
        importCmd.AddOption(fileOption);
        importCmd.AddOption(databaseOption);
        
        importCmd.SetHandler(async (file, database) =>
        {
            var importer = new ImportGames();
            await importer.ImportGameAsync(file, database);
        },fileOption, databaseOption);
        
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
}