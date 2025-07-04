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
		{
			"knight middle of the board no blockers",
			"Nd5",
			Knight,
			WhitePiece,
			"d5",
			"Nd5f4 Nd5f6 Nd5e7 Nd5e3 Nd5c3 Nd5c7 Nd5b4 Nd5b6",
		},
		{
			"queen middle of the board no blockers",
			"Qd5",
			Queen,
			WhitePiece,
			"d5",
			"Qd5e5 Qd5f5 Qd5g5 Qd5h5 Qd5c5 Qd5b5 Qd5a5 Qd5d6 Qd5d7 Qd5d8 Qd5d4 Qd5d3 Qd5d2 Qd5d1 Qd5e6 Qd5f7 Qd5g8 Qd5e4 Qd5f3 Qd5g2 Qd5h1 Qd5c6 Qd5b7 Qd5a8 Qd5c4 Qd5b3 Qd5a2",
		},
		{
			"white pawn middle of the board no blockers",
			"Pd5",
			Pawn,
			WhitePiece,
			"d5",
			"Pd5d6",
		},
		{
			"black pawn middle of the board no blockers",
			"pd5",
			Pawn,
			BlackPiece,
			"d5",
			"pd5d4",
		},
		{
			"white pawn starting square no blockers",
			"Pd2",
			Pawn,
			WhitePiece,
			"d2",
			"Pd2d3 Pd2d4",
		},
		{
			"black pawn starting square no blockers",
			"pd7",
			Pawn,
			BlackPiece,
			"d7",
			"pd7d5 pd7d6",
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
