package chess

import (
	"fmt"
	game "github.com/jerhon/chess/pkg/chess/game"
	"github.com/jerhon/chess/pkg/chess/san"
)

type ChessGame struct {
	position *game.ChessPosition
	moves    *game.ChessMovement
}

func NewGame() *ChessGame {

	position := game.NewStandardStartingPosition()

	moves := game.NewChessMovement(position)
	moves.Calculate()

	return &ChessGame{
		position,
		moves,
	}
}

func (g *ChessGame) calculate() {
	if g.moves == nil || g.moves.Position != g.position {
		g.moves = game.NewChessMovement(g.position)
		g.moves.Calculate()
	}
}

func (g *ChessGame) castleKingSide() {
	g.position.CastleKingside()
	g.calculate()
}

func (g *ChessGame) castleQueenSide() {
	g.position.CastleQueenside()
	g.calculate()
}

func (g *ChessGame) TrySanMove(sanText string) (bool, error) {

	if g.moves.IsCheckmate {
		return false, fmt.Errorf("Game is checkmate")
	}
	if g.moves.IsStalemate {
		return false, fmt.Errorf("Game is stalemate")
	}

	sanMove, sanCastle, err := san.ParseSan(sanText)
	if err != nil {
		return false, fmt.Errorf("Invalid SAN %s: %s", err)
	}

	if sanCastle != nil {
		if sanCastle.CastleKingSide && !g.moves.CanCastle.KingSide {
			return false, fmt.Errorf("Cannot castle king side")
		}

		if sanCastle.CastleQueenSide && !g.moves.CanCastle.QueenSide {
			return false, fmt.Errorf("Cannot castle queen side")
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

			fromPiece, _ := g.position.Board.GetPiece(move.From.Location)
			if fromPiece.Color != g.position.PlayerToMove {
				continue
			}

			actualMoves = append(actualMoves, move)
		}

		if len(actualMoves) != 1 {
			return false, fmt.Errorf("Invalid move.")
		}

		move := actualMoves[0]
		newPosition := g.position.Move(move.From.Location, move.To)

		g.position = newPosition
		g.calculate()
	} else {
		return false, fmt.Errorf("Invalid SAN %s", sanText)
	}

	return true, nil
}

func (g *ChessGame) GetPosition() *game.ChessPosition {
	return g.position
}
