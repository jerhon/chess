using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Honlsoft.Chess.Rules;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame : IChessGame {
    private readonly GameRules _rules;

    private readonly List<PlayerTurn> _gameMoves = new();
    private readonly ChessPositionBuilder _chessPosition;
    
    public PieceColor CurrentPlayer { get; private set; } = PieceColor.White;

    public IChessPosition CurrentPosition => _chessPosition;
    
    public SquareName? EnPassantTarget { get; private set; }

    public ChessGameState GameState { get; private set; }

    
    public ChessGame(IChessPosition initialChessPosition, PieceColor playerToMove,  GameRules rules) {
        _rules = rules;

        _chessPosition = new ChessPositionBuilder()
            .FromBoard(initialChessPosition);
        
        GameState = rules.CalculateState(initialChessPosition, playerToMove);
        
        CurrentPlayer = playerToMove;
    }

    public ChessGame(GameRules gameRules) : this(ChessPositionBuilder.StandardGame, PieceColor.White, gameRules) { }
    
    /// <summary>
    /// Trys to move a piece in the game.
    /// </summary>
    /// <param name="fromName">The position to move from</param>
    /// <param name="toName">The position to move to</param>
    /// <returns></returns>
    public MoveResult Move(SquareName fromName, SquareName toName, PieceType? promotionPiece) {
        
        var (validationResult, move) = _rules.IsValidMove(this, fromName, toName, promotionPiece);
        if (move == null || validationResult != MoveResult.ValidMove) {
            return validationResult;
        }

        DoHalfMoves(fromName, toName);
        
        DoCastlingRights(fromName);
        
        DoEnPassantTarget(fromName, toName);
        
        // Move the piece
        _chessPosition.Move(fromName, toName);

        DoEnPassantCapture(fromName, toName);
        
        // If this is a promotion, then promote it
        if (promotionPiece != null) {
            _chessPosition.SetSquare(toName, promotionPiece.Value, CurrentPlayer);
        }
        
        // Full move counter increments when black moves
        if (CurrentPlayer == PieceColor.Black) {
            _chessPosition.IncrementFullMoves();
        }
        
        // Change the color
        _chessPosition.SwitchColor();

        // Calculate the current state of the game
        GameState = _rules.CalculateState(_chessPosition, CurrentPlayer);
        
        // Record the turn
        _gameMoves.Add(new PlayerTurn(_chessPosition.Build(), move, CurrentPlayer));
        
        
        return validationResult;
    }
    

    public MoveResult Castle(CastlingSide side) {
        
        if (!_chessPosition.CanCastle(_chessPosition.PlayerToMove, side)) {
            return MoveResult.CastlingNotAllowed;
        }

        // TODO: add validation that squares are empty...
        
        SquareRank squareRank = _chessPosition.PlayerToMove is PieceColor.Black ? SquareRank.Rank8 : SquareRank.Rank1;
        SquareFile rookSquareFile = side is CastlingSide.Queenside ? SquareFile.a : SquareFile.h;

        SquareFile rookFinalSquareFile = side is CastlingSide.Queenside ? SquareFile.d : SquareFile.f;
        SquareFile kingFinalSquareFile = side is CastlingSide.Queenside ? SquareFile.c : SquareFile.g;
        
        SquareName kingSquare = new SquareName(SquareFile.e, squareRank);
        SquareName rookSquare = new SquareName(rookSquareFile, squareRank);
        SquareName kingToSquare = new SquareName(kingFinalSquareFile, squareRank);
        SquareName rookToSquare = new SquareName(rookFinalSquareFile, squareRank);

        _chessPosition.Move(kingSquare, kingToSquare)
                .Move(rookSquare, rookToSquare)
                .WithCastlingRights(_chessPosition.PlayerToMove, side, false);

        _chessPosition.IncrementHalfMoves();

        if (_chessPosition.PlayerToMove == PieceColor.Black) {
            _chessPosition.IncrementFullMoves();
        }

        return MoveResult.ValidMove;
    }
    
    
    
    private void DoCastlingRights(SquareName from) {
        if (_chessPosition.IsKing(from)) {
            _chessPosition.RemoveCastlingRights(CastlingSide.Kingside);
            _chessPosition.RemoveCastlingRights(CastlingSide.Queenside);
        }
        if (_chessPosition.IsQueensideRook(from)) {
            _chessPosition.RemoveCastlingRights(CastlingSide.Queenside);
        }
        if (_chessPosition.IsKingsideRook(from)) {
            _chessPosition.RemoveCastlingRights(CastlingSide.Kingside);
        }
    }

    private void DoEnPassantTarget(SquareName fromName, SquareName toName) {
        // En-passant
        if (_chessPosition.IsPawn(fromName)) {
            if (fromName.SquareRank == SquareRank.Rank2 && toName.SquareRank == SquareRank.Rank4) {
                _chessPosition.WithEnPassantTarget(fromName with { SquareRank = SquareRank.Rank3 });
                return;
            }
            else if (fromName.SquareRank == SquareRank.Rank7 && toName.SquareRank == SquareRank.Rank5) {
                _chessPosition.WithEnPassantTarget(fromName with { SquareRank = SquareRank.Rank6 });
                return;
            }
        }

        _chessPosition.WithEnPassantTarget(null);
    }

    private void DoHalfMoves(SquareName fromName, SquareName toName) {
        // Deal with the half moves
        if (_chessPosition.IsPawn(fromName)) {
            _chessPosition.ResetHalfMoves();
        } else if (_chessPosition.IsCapture(toName)) {
            _chessPosition.ResetHalfMoves();
        } else {
            _chessPosition.IncrementHalfMoves();
        }
    }

    private void DoEnPassantCapture(SquareName fromName, SquareName toName) {
        
        // If this is the en-passant target, and the moving piece is a pawn
        if (EnPassantTarget == toName && _chessPosition.IsPawn(toName)) {
            var captureSquare = new SquareName(toName.SquareFile, fromName.SquareRank);
            _chessPosition.RemovePiece(captureSquare);
            _chessPosition.WithEnPassantTarget(null);
        }
    }

    public SquareName[] GetCandidateMoves(SquareName squareName)  => _rules.GetMoves(_chessPosition, squareName).Select((m) => m.To).ToArray();
    
}