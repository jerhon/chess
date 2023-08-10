using Honlsoft.Chess.GameService.Model;
using Microsoft.AspNetCore.Mvc;

namespace Honlsoft.Chess.GameService.Controllers;


public class NewGameResult {
    public Guid Id { get; set; }
}

public class GameMove {
    
    public string From { get; set; }
    
    public string To { get; set; }
    
}

public class GameMoveResult {
    
    public bool Success { get; set; }
    
    public bool Reason { get; set; }
    
}


[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase {
    
    private readonly IChessGameStorage _chessGameStorage;

    public GameController(IChessGameStorage chessGameStorage) {
        _chessGameStorage = chessGameStorage;
    }


    /// <summary>
    /// New game created, has an id...
    /// </summary>
    /// <returns>The new game.</returns>
    [HttpPut]
    public NewGameResult NewGame() {
        
        // TODO: this will need to have match making / join ability, but for now, we're keeping it simple to test the thing
        
        var id = _chessGameStorage.NewGame();
        return new NewGameResult{ Id = id };
    }

    [HttpPut(":id/move")]
    public IResult<GameMoveResult> Move(Guid id, [FromBody]GameMove move) {

        // TODO: Need to add validation of user whether they are a player in the game, and what side they are playing
        var game = _chessGameStorage.GetGameState(id);
        if (game == null) {
            Results.NotFound();
        }

        var from = SquareName.Parse(move.From);
        var to = SquareName.Parse(move.To);

        var (canMove, result) = game.MovePiece(from, to);
        if (canMove) {
            return new GameMoveResult()    
        }
    }
    
}