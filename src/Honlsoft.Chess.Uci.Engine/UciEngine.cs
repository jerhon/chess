using Honlsoft.Chess.Engine;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;
using Honlsoft.Chess.Uci.Client;

namespace Honlsoft.Chess.Uci.Engine;

public class UciEngine : IChessEngine {

    private UciClient _client;
    
    public UciEngine(UciClient client) {
        _client = client;
    }


    public async Task StartGameAsync(IChessBoard chessBoard, ChessMove[] moves, CancellationToken cancellationToken) {

        FenSerializer fenSerializer = new FenSerializer();
        string fenString = fenSerializer.Serialize(chessBoard);

        await _client.UciNewGameAsync(cancellationToken);
        await _client.SetFenPositionAsync(fenString, moves.Select((m) => m.ToString()).ToArray(), cancellationToken);
    }
    
    public Task SendMoveAsync(ChessMove move, CancellationToken cancelToken) {
        throw new NotImplementedException();
    }
    
    public async Task<ChessMove> SuggestMoveAsync(CancellationToken cancelToken) {
        var bestMove = await _client.GoAsync(new GoParameters(), cancelToken);

        return MapUciMoveToChessMove(bestMove.Move);
    }

    private ChessMove MapUciMoveToChessMove(string uciMove) {
        string firstPosition = uciMove.Substring(0, 2);
        string secondPosition = uciMove.Substring(2, 2);
        
        SquareName startingSquare = SquareName.Parse(firstPosition);
        SquareName endingSquare = SquareName.Parse(secondPosition);

        return new ChessMove(startingSquare, endingSquare);
    }
}