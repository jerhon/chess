// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Honlsoft.Chess;
using Honlsoft.Chess.Console;
using Honlsoft.Chess.Console.CommandLine;
using Honlsoft.Chess.Console.UseCases;
using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;
using Honlsoft.Chess.Uci.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Spectre.Console;



Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Honlsoft", LogEventLevel.Verbose)
    .WriteTo.File("C:\\logs\\honlsoft.chess.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

ServiceCollection collection = new ServiceCollection();
collection.AddLogging((builder) => {
    builder.ClearProviders();
    builder.AddSerilog();
});

collection.AddSingleton<PlayGame>();
collection.AddSingleton<CommandLineFactory>();
collection.AddSingleton<ImportGames>();

var serviceProvider = collection.BuildServiceProvider();
var commandLine = serviceProvider.GetRequiredService<CommandLineFactory>();

var root = commandLine.CreateCommand();

await root.InvokeAsync(args);