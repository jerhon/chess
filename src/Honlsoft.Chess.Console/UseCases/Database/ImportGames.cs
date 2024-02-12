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
        var chessMatch = serializer.Deserialize(rawPgnText);
        
        
        ChessGameFactory gameFactory = new ChessGameFactory();
        ChessGame game = gameFactory.CreateStandardGame();

        Game dataGame = new Game();
        dataGame.Pgn = rawPgnText;
        
        var fenSerializer = new FenSerializer();
        var zorbistHasher = new ZorbistHasher();
        
        System.Console.WriteLine("Total Moves: " + chessMatch.Moves.Length);
        
        ulong previousPositionHash = zorbistHasher.CalculateHash(game.CurrentPosition);
        
        using var context = new ChessContext(optionsBuilder.Options);
        
        
        await context.Database.EnsureCreatedAsync();

        var fenOptions = FenParts.Castling | FenParts.Positions | FenParts.EnPassant | FenParts.MoveTurn;
        
        // Perform each move, and output the position.
        // May make sense to cache some of the chess positions in memory to avoid database lookups.
        foreach (var move in chessMatch.Moves)
        {
            // System.Console.WriteLine(move.Move);
            
            var initialFen = fenSerializer.Serialize(game.CurrentPosition, fenOptions);

            System.Console.WriteLine($"{move.MoveNumber} {move.Move} - {initialFen}");
            
            var moveResult = game.Move(move.Move);
            if (moveResult != MoveResult.ValidMove)
            {
                throw new InvalidOperationException($"Move {move.Move} invalid. Reason = {moveResult}.");
            }
         
            var hash = zorbistHasher.CalculateHash(game.CurrentPosition);
            
            var fen = fenSerializer.Serialize(game.CurrentPosition, fenOptions);

            // Find matching positions.
            var chessPositions = await context.ChessPositions.Where((cp) => cp.Hash == hash).ToArrayAsync();
            ChessPosition dataPosition = chessPositions.FirstOrDefault((cp) => cp.Fen == fen);
            if (chessPositions.Length == 0)
            {
                dataPosition = new ChessPosition()
                {
                    Fen = fen,
                    Hash = hash,
                    EnPassantTarget = game.CurrentPosition.EnPassantTarget?.ToString(),
                    WhiteCanCastleKingSide = game.CurrentPosition.CanCastle(PieceColor.White, CastlingSide.Kingside),
                    WhiteCanCastleQueenSide = game.CurrentPosition.CanCastle(PieceColor.White, CastlingSide.Queenside),
                    BlackCanCastleKingSide = game.CurrentPosition.CanCastle(PieceColor.Black, CastlingSide.Kingside),
                    BlackCanCastleQueenSide = game.CurrentPosition.CanCastle(PieceColor.Black, CastlingSide.Queenside)
                };
                context.ChessPositions.Add(dataPosition);
            }
            
            var position = new GamePosition()
            {
                Game = dataGame,
                PreviousHash = previousPositionHash,
                Hash = hash,
                MoveNumber = move.MoveNumber,
                PlayerToMove = MapToDatabaseColor(game.CurrentPosition.PlayerToMove),
                Position = dataPosition,
                HalfMoves = game.CurrentPosition.HalfMoves,
                FullMoves = game.CurrentPosition.FullMoves,
            };
            
            previousPositionHash = hash;
            
            dataGame.Positions.Add(position);
        }
        
        
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