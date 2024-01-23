using System.Threading.Channels;
using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Engine;

/// <summary>
/// Provides a chess engine backed by a UCI interface.
/// </summary>
public class UciEngine(ChessGame chessGame, UciClient client) : IChessEngine {

    private List<string> _moves = new List<string>();
    private Channel<UciCommand> _commandChannel = Channel.CreateUnbounded<UciCommand>();
    private bool _calculating = false;
    
    /// <summary>
    /// Runs initialization for the underlying chess engine.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeEngineAsync(CancellationToken cancellationToken) {
        await client.UciAsync(cancellationToken);
        await client.IsReadyAsync(cancellationToken);
    }
    
    public async Task StartGameAsync(CancellationToken cancellationToken) {

        FenSerializer fenSerializer = new FenSerializer();
        string fenString = fenSerializer.Serialize(chessGame.CurrentPosition);

        await client.UciNewGameAsync(cancellationToken);
        await client.SetFenPositionAsync(fenString, [], cancellationToken);
    }

    public void AddMove(string moveString) 
    {
        _moves.Add(moveString);
    }
    
    public async Task UpdatePositionAsync(CancellationToken cancellationToken) {
        await client.SetStartingPositionAsync(_moves.ToArray(), cancellationToken);
        await client.IsReadyAsync(cancellationToken);
    }
    
    Task<Channel<EngineSuggestion>> IChessEngine.StartCalculatingAsync(CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
    
    public Task<BestMove> StopCalculatingAsync(CancellationToken cancelToken) {
        throw new NotImplementedException();
    }

    public async Task<Channel<EngineSuggestion>> StartCalculatingAsync(CancellationToken cancelToken) {

        var commandChannel = await client.GoAsync(new GoParameters(), cancelToken);
        Channel<EngineSuggestion> suggestionChannel = Channel.CreateUnbounded<EngineSuggestion>();

        Task.Run(async () => {
            
            
            UciCommand? bestMove = null;
            while (await commandChannel.Reader.WaitToReadAsync(cancelToken)) {
                var command = await commandChannel.Reader.ReadAsync(cancelToken);
                
                
                var engineSuggestion = new EngineSuggestion( )
                
                suggestionChannel.Writer.TryWrite(MapUciMoveToChessMove(command.GetParameter("bestmove")!));
                
            }
            
            suggestionChannel.Writer.Complete();

        });
        
        if (_calculating) {
            

            Channel<EngineSuggestion> channel = Channel 
            
            

            return MapUciMoveToChessMove(bestMove!.Parameters[0].Value!);
        }
    }

    public static EngineSuggestion MapUciMoveToChessMove(string uciMove) {
        string firstPosition = uciMove.Substring(0, 2);
        string secondPosition = uciMove.Substring(2, 2);
        
        SquareName startingSquare = SquareName.Parse(firstPosition);
        SquareName endingSquare = SquareName.Parse(secondPosition);

        return new EngineSuggestion(startingSquare, endingSquare);
    }

    public static string MapChessMoveToUciMove(EngineSuggestion chessMove) {
        var from = chessMove.From.ToString();
        var to = chessMove.To.ToString();

        return "{from}{to}";
    }
}