using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;

namespace Honlsoft.Chess.Uci.Engine;

/// <summary>
/// Provides a chess engine backed by a UCI interface.
/// </summary>
public class UciEngine(ChessGame chessGame, UciClient client) : IChessEngine {


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
        string fenString = fenSerializer.Serialize(chessGame.CurrentBoard);

        await client.UciNewGameAsync(cancellationToken);
        await client.SetFenPositionAsync(fenString, [], cancellationToken);
    }
    
    public Task SendMoveAsync(ChessMove move, CancellationToken cancelToken) {
        var uciMove = MapChessMoveToUciMove(move);
        return client.SetMovePositionAsync([uciMove], cancelToken);
    }
    
    public async Task<ChessMove> SuggestMoveAsync(CancellationToken cancelToken) {
        // TODO: Need to send current position
        
        
        var bestMove = await client.GoAsync(new GoParameters(), cancelToken);

        return MapUciMoveToChessMove(bestMove.Move);
    }

    public static ChessMove MapUciMoveToChessMove(string uciMove) {
        string firstPosition = uciMove.Substring(0, 2);
        string secondPosition = uciMove.Substring(2, 2);
        
        SquareName startingSquare = SquareName.Parse(firstPosition);
        SquareName endingSquare = SquareName.Parse(secondPosition);

        return new ChessMove(startingSquare, endingSquare);
    }

    public static string MapChessMoveToUciMove(ChessMove chessMove) {
        var from = chessMove.From.ToString();
        var to = chessMove.To.ToString();

        return "{from}{to}";
    }
}