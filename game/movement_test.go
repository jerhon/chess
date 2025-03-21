package game

import (
	"github.com/stretchr/testify/assert"
	"strings"
	"testing"
)

func TestCalculateMoves(t *testing.T) {
	tests := []struct {
		name         string
		boardSetup   string
		pieceType    PieceType
		playerToMove ColorType
		fromSquare   string
		toSquares    string
	}{
		{
			"bishop middle of the board no blockers",
			"Bd5",
			Bishop,
			WhitePiece,
			"d5",
			// a b c d e f g h
			"Bd5e6 Bd5f7 Bd5g8 Bd5e4 Bd5f3 Bd5g2 Bd5h1 Bd5c6 Bd5b7 Bd5a8 Bd5c4 Bd5b3 Bd5a2",
		},
		{
			"rook middle of the board no blockers",
			"Rd5",
			Rook,
			WhitePiece,
			"d5",
			"Rd5e5 Rd5f5 Rd5g5 Rd5h5 Rd5c5 Rd5b5 Rd5a5 Rd5d6 Rd5d7 Rd5d8 Rd5d4 Rd5d3 Rd5d2 Rd5d1",
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := parseBoard(test.boardSetup)
			position := &ChessPosition{Board: board, PlayerToMove: test.playerToMove}
			fromSquare := ParseChessLocation(test.fromSquare)
			toSquares := strings.Split(test.toSquares, " ")
			actualMoves := getMoveStrings(CalculateMoves(position, fromSquare))
			assert.ElementsMatch(t, toSquares, actualMoves)
		})
	}
}

func getMoveStrings(moves []ChessMove) []string {
	stringMoves := []string{}
	for _, move := range moves {
		stringMoves = append(stringMoves, move.String())
	}
	return stringMoves
}

func parseBoard(squareLayoutString string) *ChessBoard {
	board := NewChessBoard()
	squares := strings.Split(squareLayoutString, " ")
	for _, squareString := range squares {
		square := ParseSquare(squareString)
		board.SetSquare(square.Location, square.Piece)
	}
	return board
}
