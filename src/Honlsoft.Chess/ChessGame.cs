using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Honlsoft.Chess.Rules;
using Honlsoft.Chess.Serialization;


// TODO: Promotion, En passant, Stalemate Due to Repetition, Stalemate Due to Move Count, others?

namespace Honlsoft.Chess; 

/// <summary>
/// Engine for a standard chess game.
/// </summary>
public class ChessGame : IChessGame {
    private readonly GameRules _rules;

    private readonly List<PlayerTurn> _gameMoves = new();
    private readonly ChessPositionBuilder _chessPosition;
    
    public IChessPosition CurrentPosition => _chessPosition;

    public ChessGameState GameState { get; private set; }

    
    public ChessGame(IChessPosition initialChessPosition,  GameRules rules) {
        _rules = rules;

        _chessPosition = new ChessPositionBuilder()
            .FromPosition(initialChessPosition);
        
        GameState = rules.CalculateState(initialChessPosition);
        
    }

    public ChessGame(GameRules gameRules) : this(ChessPositionBuilder.StandardGame, gameRules) { }
    
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
        
        DoEnPassant(fromName, toName);
        
        // Move the piece
        _chessPosition.Move(fromName, toName);
        
        // If this is a promotion, then promote it
        if (promotionPiece != null) {
            _chessPosition.SetSquare(toName, promotionPiece.Value, _chessPosition.PlayerToMove);
        }
        
        // Full move counter increments when black moves
        if (_chessPosition.PlayerToMove == PieceColor.Black) {
            _chessPosition.IncrementFullMoves();
        }
        
        // Change the color
        _chessPosition.SwitchColor();

        // Calculate the current state of the game
        GameState = _rules.CalculateState(_chessPosition);
        
        // Record the turn
        _gameMoves.Add(new PlayerTurn(_chessPosition.Build(), move, _chessPosition.PlayerToMove));
        
        
        return validationResult;
    }


    /// <summary>
    /// Moves a piece from a SAN move description.
    /// </summary>
    /// <param name="sanMove"></param>
    /// <returns></returns>
    public MoveResult Move(San san) {

        if (san == null)
        {
            throw new ArgumentNullException(nameof(san));
        }
        
        if (san is SanCastle sanCastle) 
        {
            return this.Castle(sanCastle.Side);
        }
        else if (san is SanMove sanMove)
        {
            var chessMove = GetMoveFromSan(sanMove);
            if (chessMove == null) {
                return MoveResult.NotALegalMove;
            }
            return Move(chessMove.From, chessMove.To, sanMove.PromotionPiece);
        }
        else
        {
            throw new ArgumentException("The type of San move passed in cannot be used as a move. Type = " + san.GetType().Name, nameof(san));
        }
    }

    public MoveResult Castle(CastlingSide side) {
        
        if (!_chessPosition.CanCastle(_chessPosition.PlayerToMove, side)) {
            return MoveResult.CastlingNotAllowed;
        }

        SquareRank squareRank = _chessPosition.PlayerToMove is PieceColor.Black ? SquareRank.Rank8 : SquareRank.Rank1;
        SquareFile rookSquareFile = side is CastlingSide.Queenside ? SquareFile.a : SquareFile.h;

        SquareFile rookFinalSquareFile = side is CastlingSide.Queenside ? SquareFile.d : SquareFile.f;
        SquareFile kingFinalSquareFile = side is CastlingSide.Queenside ? SquareFile.c : SquareFile.g;
        
        SquareName kingSquare = new SquareName(SquareFile.e, squareRank);
        SquareName rookSquare = new SquareName(rookSquareFile, squareRank);
        SquareName kingToSquare = new SquareName(kingFinalSquareFile, squareRank);
        SquareName rookToSquare = new SquareName(rookFinalSquareFile, squareRank);

        if (_chessPosition.GetSquare(kingToSquare).HasPiece) {
            return MoveResult.NotALegalMove;
        }
        if (_chessPosition.GetSquare(rookToSquare).HasPiece) {
            return MoveResult.NotALegalMove;
        }

        _chessPosition.Move(kingSquare, kingToSquare)
            .WithEnPassantTarget(null)
                .Move(rookSquare, rookToSquare)
                .RemoveCastlingRights(_chessPosition.PlayerToMove);
        
        _chessPosition.IncrementHalfMoves();

        if (_chessPosition.PlayerToMove == PieceColor.Black) {
            _chessPosition.IncrementFullMoves();
        }

        _chessPosition.SwitchColor();
        
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

    private void DoEnPassant(SquareName fromName, SquareName toName) {
       
        var oldEnPassant = _chessPosition.EnPassantTarget; 
       
        
        if (_chessPosition.IsPawn(fromName)) {
            // En-passant
            if (fromName.SquareRank == SquareRank.Rank2 && toName.SquareRank == SquareRank.Rank4) {
                _chessPosition.WithEnPassantTarget(fromName with { SquareRank = SquareRank.Rank3 });
                return;
            }
            else if (fromName.SquareRank == SquareRank.Rank7 && toName.SquareRank == SquareRank.Rank5) {
                _chessPosition.WithEnPassantTarget(fromName with { SquareRank = SquareRank.Rank6 });
                return;
            }
            else if (oldEnPassant == toName) {
               var captureSquare = new SquareName(toName.SquareFile, fromName.SquareRank);
               _chessPosition.RemovePiece(captureSquare); 
            }
        }

        _chessPosition.WithEnPassantTarget(null);
    }

    public IChessMove? GetMoveFromSan(San san) {
        
        var playerSquares = SquareName.AllSquares().Select((s) => _chessPosition.GetSquare(s))
            .Where((s) => s.Piece?.Color == _chessPosition.PlayerToMove);
        
        if (san is SanMove sanMove) {

            // If it narrows down by piece, use that
            if (sanMove.FromPiece != null) {
                playerSquares = playerSquares.Where((ps) => ps.Piece?.Type == sanMove.FromPiece);
            }
                    
            var candidateMoves = playerSquares.SelectMany((s) => _rules.GetMoves(_chessPosition, s.Name));
            
            IEnumerable<IChessMove> possibleMoves = candidateMoves.Where((m) => m.To.SquareFile == sanMove.ToFile && m.To.SquareRank == sanMove.ToRank).ToArray();
            if (!possibleMoves.Any())
            {
                FenSerializer serializer = new();
                var fen = serializer.Serialize(_chessPosition);
                throw new InvalidOperationException($"Invalid move. Move = {san} FEN = {fen}");
            }
            else if (possibleMoves.Take(2).Count() == 1) {
                var move = possibleMoves.First();
                return move;
            } else {
                if (sanMove.FromFile != null) {
                    possibleMoves = possibleMoves.Where((pm) => pm.From.SquareFile == sanMove.FromFile);
                }
                if (possibleMoves.Take(2).Count() == 1) {
                    return possibleMoves.First();
                }
                if (sanMove.FromRank != null) {
                    possibleMoves = possibleMoves.Where((pm) => pm.From.SquareRank == sanMove.FromRank);
                }
                if (possibleMoves.Take(2).Count() == 1) {
                    return possibleMoves.First();
                }
                if (sanMove.FromPiece != null) {
                    
                    possibleMoves = possibleMoves.Where((pm) =>
                    {
                        var square = _chessPosition.GetSquare(pm.From);
                        if (square.Piece?.Type == sanMove.FromPiece) {
                            return true;
                        }

                        return false;
                    });
                }
                if (possibleMoves.Take(2).Count() == 1)
                {
                    return possibleMoves.First();
                }

                // If the piece isn't included, should assume it's a pawn
                if (sanMove.FromPiece == null)
                {
                    possibleMoves = possibleMoves.Where((pm) =>
                                        {
                                            var square = _chessPosition.GetSquare(pm.From);
                                            if (square.Piece?.Type == PieceType.Pawn) {
                                                return true;
                                            }
                    
                                            return false;
                                        });
                }
                
                if (possibleMoves.Take(2).Count() == 1)
                {
                    return possibleMoves.First();
                }
            }
            // Couldn't find a move from the Standard Algebraic Notation
            return null;
        }
        
        
        throw new InvalidOperationException("Unsupported SAN type.");
    }

    public San[] GetCandidateMoves(SquareName squareName)  => _rules.GetMoves(_chessPosition, squareName).Select((m) => m.ToSan()).ToArray();
    
}