package chess

import (
	"fmt"
	"strings"

	"github.com/jerhon/chess/pkg/chess/fen"
	game "github.com/jerhon/chess/pkg/chess/game"
	"github.com/jerhon/chess/pkg/chess/san"
)

type ChessGame struct {
	position        *game.ChessPosition
	moves           *game.ChessMovement
	positionHistory map[string]int
}

// positionKey returns a string key for the given position that captures all factors
// relevant to threefold repetition: board state, player to move, castling rights,
// and en passant square. It uses the first four fields of the FEN string and omits
// the halfmove clock and fullmove number, which do not affect position identity
// for repetition purposes under standard chess rules.
func positionKey(position *game.ChessPosition) string {
	fenStr := fen.ToFenString(position)
	parts := strings.Fields(fenStr)
	if len(parts) >= 4 {
		return strings.Join(parts[:4], " ")
	}
	return fenStr
}

func NewGame() *ChessGame {

	position := game.NewStandardStartingPosition()

	moves := game.NewChessMovement(position)
	moves.Calculate()

	return &ChessGame{
		position:        position,
		moves:           moves,
		positionHistory: map[string]int{positionKey(position): 1},
	}
}

// NewGameFromPosition creates a new ChessGame starting from the given position.
func NewGameFromPosition(position *game.ChessPosition) *ChessGame {
	moves := game.NewChessMovement(position)
	moves.Calculate()

	return &ChessGame{
		position:        position,
		moves:           moves,
		positionHistory: map[string]int{positionKey(position): 1},
	}
}

func (g *ChessGame) calculate() {
	if g.moves == nil || g.moves.Position != g.position {
		g.moves = game.NewChessMovement(g.position)
		g.moves.Calculate()
	}
}

// recordCurrentPosition increments the visit count for the current position and
// declares DrawRepetition when the same position has occurred three or more times.
func (g *ChessGame) recordCurrentPosition() {
	key := positionKey(g.position)
	g.positionHistory[key]++
	if g.moves.Result == game.InProgress && g.positionHistory[key] >= 3 {
		g.moves.Result = game.DrawRepetition
	}
}

func (g *ChessGame) castleKingSide() {
	g.position = g.position.CastleKingside()
	g.calculate()
}

func (g *ChessGame) castleQueenSide() {
	g.position = g.position.CastleQueenside()
	g.calculate()
}

func (g *ChessGame) TrySanMove(sanText string) (bool, error) {

	if g.moves.Result != game.InProgress {
		return false, fmt.Errorf("game is over: %s", g.moves.Result)
	}

	sanMove, sanCastle, err := san.ParseSan(sanText)
	if err != nil {
		return false, fmt.Errorf("invalid SAN %s: %v", sanText, err)
	}

	if sanCastle != nil {
		if sanCastle.CastleKingSide && !g.moves.CanCastle.KingSide {
			return false, fmt.Errorf("cannot castle king side")
		}

		if sanCastle.CastleQueenSide && !g.moves.CanCastle.QueenSide {
			return false, fmt.Errorf("cannot castle queen side")
		}

		if sanCastle.CastleKingSide {
			g.castleKingSide()
		}

		if sanCastle.CastleQueenSide {
			g.castleQueenSide()
		}

	} else if sanMove != nil {

		// find the appropriate move from the list of moves based on the san notation
		actualMoves := []game.ChessMove{}
		for _, move := range g.moves.Moves {
			if sanMove.ToFile != move.To.File || sanMove.ToRank != move.To.Rank {
				continue
			}

			if sanMove.FromRank != game.NoRank && move.From.Location.Rank != sanMove.FromRank {
				continue
			}

			if sanMove.FromFile != game.NoFile && move.From.Location.File != sanMove.FromFile {
				continue
			}

			if sanMove.Piece != game.NoPiece && move.From.Piece.Piece != sanMove.Piece {
				continue
			}

			// A promotion move requires a promotion piece in the SAN; a non-promotion move must not have one.
			if move.IsPromotion != (sanMove.PromotionPiece != game.NoPiece) {
				continue
			}

			fromPiece, _ := g.position.Board.GetPiece(move.From.Location)
			if fromPiece.Color != g.position.PlayerToMove {
				continue
			}

			actualMoves = append(actualMoves, move)
		}

		if len(actualMoves) != 1 {
			return false, fmt.Errorf("invalid move, multiple pieces can move to the same square: %v", actualMoves)
		}

		move := actualMoves[0]
		promotionPiece := game.NoPiece
		if move.IsPromotion {
			switch sanMove.PromotionPiece {
			case game.Queen, game.Rook, game.Bishop, game.Knight:
				promotionPiece = sanMove.PromotionPiece
			default:
				return false, fmt.Errorf("invalid promotion piece %v in SAN %s", sanMove.PromotionPiece, sanText)
			}
		}
		newPosition := g.position.Move(move.From.Location, move.To, promotionPiece)

		g.position = newPosition
		g.calculate()
	} else {
		return false, fmt.Errorf("invalid SAN %s", sanText)
	}

	g.recordCurrentPosition()
	return true, nil
}

func (g *ChessGame) GetPosition() *game.ChessPosition {
	return g.position
}

// GetMoves returns the list of valid moves for the current player.
func (g *ChessGame) GetMoves() []game.ChessMove {
	g.calculate()
	return g.moves.Moves
}

// GetLegalMoves returns all legal moves available for the current player in the current position.
// Only moves that can actually be executed are returned (moves where CanMove is true, or castling moves).
func (g *ChessGame) GetLegalMoves() []game.ChessMove {
	g.calculate()
	legalMoves := make([]game.ChessMove, 0, len(g.moves.Moves))
	for _, move := range g.moves.Moves {
		if move.CanMove || move.IsCastle {
			legalMoves = append(legalMoves, move)
		}
	}
	return legalMoves
}

// IsCheckmate returns true if the current player is in checkmate.
func (g *ChessGame) IsCheckmate() bool {
	g.calculate()
	return g.moves.IsCheckmate
}

// IsStalemate returns true if the current player is in stalemate.
func (g *ChessGame) IsStalemate() bool {
	g.calculate()
	return g.moves.IsStalemate
}

// IsCheck returns true if the current player is in check.
func (g *ChessGame) IsCheck() bool {
	g.calculate()
	return g.moves.Check[g.position.PlayerToMove]
}

// GetResult returns the current result of the game.
// It returns InProgress if the game is still ongoing.
func (g *ChessGame) GetResult() game.GameResult {
	g.calculate()
	return g.moves.Result
}
