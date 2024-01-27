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

    private List<San> _moves = new List<San>();
    private Channel<UciCommand> _commandChannel = Channel.CreateUnbounded<UciCommand>();
    private bool _calculating = false;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private TaskCompletionSource<UciCommand> _bestMove;
    
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

    public void MakeMove(San sanMove) 
    {
        _moves.Add(sanMove);
    }
    
    public async Task UpdatePositionAsync(CancellationToken cancellationToken) {
        await client.SetStartingPositionAsync(_moves.ToArray(), cancellationToken);
        await client.IsReadyAsync(cancellationToken);
    }
    
    public async Task<BestMove> StopCalculatingAsync(CancellationToken cancelToken)
    {
        var bestMove = await _bestMove.Task;
        var move = bestMove.Parameters[0];
        var sanSerializer = new SanSerializer();
        var san = sanSerializer.Deserialize(move.Value);
        
        return new BestMove(san);
    }

    public async Task<Channel<EngineLine>> StartCalculatingAsync(CancellationToken cancelToken) {

        if (_calculating)
        {
            throw new InvalidOperationException("Already calculating.");
        }

        _calculating = true;
        
        var commandChannel = await client.GoAsync(new GoParameters(), cancelToken);
        Channel<EngineLine> engineLineChannel = Channel.CreateUnbounded<EngineLine>();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(async () => {
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            while (await commandChannel.Reader.WaitToReadAsync(cancelToken)) {
                var command = await commandChannel.Reader.ReadAsync(cancelToken);
                if (command.Command == "info")
                {
                    var uciInfo = new UciInfo(command);
                    if (uciInfo.CentiPawns.HasValue && uciInfo.CurLine != null)
                    {
                        var engineLine = new EngineLine(uciInfo.CurLine, uciInfo.CentiPawns.Value);
                        await engineLineChannel.Writer.WriteAsync(engineLine, CancellationToken.None);
                    }
                }
                // if we end up getting a best move command, then send it
                else if (command.Command == "bestmove")
                {
                    _calculating = false;
                    _bestMove.SetResult(command);
                    engineLineChannel.Writer.Complete();
                    break;
                }
            }
            engineLineChannel.Writer.Complete();
        });

        return engineLineChannel;
    }
    
}