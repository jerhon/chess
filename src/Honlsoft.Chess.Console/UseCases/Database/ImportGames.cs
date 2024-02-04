using Honlsoft.Chess.Database;
using Honlsoft.Chess.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Honlsoft.Chess.Console.UseCases;

/// <summary>
/// Imports games into a chess database.
/// </summary>
public class ImportGames
{
    
    /// <summary>
    /// Imports a game from a PGN file into a database.
    /// </summary>
    /// <param name="pgnFile"></param>
    /// <param name="sqlLiteDatabase"></param>
    /// <returns></returns>
    public async Task ImportGameAsync(FileInfo pgnFile, FileInfo sqlLiteDatabase)
    {    
        // Open the sql lite database.
        DbContextOptionsBuilder<ChessContext> optionsBuilder = new DbContextOptionsBuilder<ChessContext>();
        optionsBuilder.UseSqlite($"Data Source={sqlLiteDatabase.FullName}");
        
        var rawPgnText = File.ReadAllText(pgnFile.FullName);
        PgnSerializer serializer = new PgnSerializer();
        var chessMatch = serializer.DeserializePgnChessMatch(rawPgnText);
        
        
        ChessGameFactory gameFactory = new ChessGameFactory();
        ChessGame game = gameFactory.CreateStandardGame();

        Game dataGame = new Game();
        dataGame.Pgn = rawPgnText;
        
        var fenSerializer = new FenSerializer();
        var zorbistHasher = new ZorbistHasher();
        
        // Perform each move, and output the position.
        foreach (var move in chessMatch.Moves)
        {
            System.Console.WriteLine(move.Move);
            
            game.Move(move.Move);
         
            var hash = zorbistHasher.CalculateHash(game.CurrentPosition);
            
            var fen = fenSerializer.Serialize(game.CurrentPosition);
            
            var position = new GamePosition()
            {
                Game = dataGame,
                Fen = fen,
                Hash = hash,
                MoveNumber = move.MoveNumber,
                PlayerToMove = MapToDatabaseColor(game.CurrentPosition.PlayerToMove)
            };
            dataGame.Positions.Add(position);
        }
        
        
        using var context = new ChessContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();
        context.Games.Add(dataGame);
        context.GamePositions.AddRange(dataGame.Positions);
        await context.SaveChangesAsync();
    }
    
    
    private Honlsoft.Chess.Database.PieceColor MapToDatabaseColor(PieceColor color)
    {
        return color switch
        {
            PieceColor.White => Honlsoft.Chess.Database.PieceColor.White,
            PieceColor.Black => Honlsoft.Chess.Database.PieceColor.Black,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
}