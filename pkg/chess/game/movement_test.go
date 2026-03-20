package game

import (
	"strings"
	"testing"

	"github.com/stretchr/testify/assert"
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
			"Nd5xf4 Nd5xf6 Nd5xe7 Nd5xe3 Nd5xc3 Nd5xc7 Nd5xb4 Nd5xb6",
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
			"Pd5d6 Pd5xe6 Pd5xc6",
		},
		{
			"black pawn middle of the board no blockers",
			"pd5",
			Pawn,
			BlackPiece,
			"d5",
			"pd5d4 pd5xe4 pd5xc4",
		},
		{
			"white pawn starting square no blockers",
			"Pd2",
			Pawn,
			WhitePiece,
			"d2",
			"Pd2d3 Pd2d4 Pd2xe3 Pd2xc3",
		},
		{
			"black pawn starting square no blockers",
			"pd7",
			Pawn,
			BlackPiece,
			"d7",
			"pd7d5 pd7d6 pd7xe6 pd7xc6",
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

func TestKingSideCastlingPathBlocked(t *testing.T) {
	tests := []struct {
		name         string
		boardSetup   string
		playerToMove ColorType
		canCastle    bool
	}{
		{
			name:         "white king-side castling blocked by piece on g1",
			boardSetup:   "Ke1 Re8 Rh1 Bg1 ke8",
			playerToMove: WhitePiece,
			canCastle:    false,
		},
		{
			name:         "white king-side castling blocked by piece on f1",
			boardSetup:   "Ke1 Rh1 Bf1 ke8",
			playerToMove: WhitePiece,
			canCastle:    false,
		},
		{
			name:         "white king-side castling clear path",
			boardSetup:   "Ke1 Rh1 ke8",
			playerToMove: WhitePiece,
			canCastle:    true,
		},
		{
			name:         "black king-side castling blocked by piece on g8",
			boardSetup:   "ke8 rh8 bg8 Ke1",
			playerToMove: BlackPiece,
			canCastle:    false,
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := parseBoard(test.boardSetup)
			position := &ChessPosition{
				Board:        board,
				PlayerToMove: test.playerToMove,
				CastlingRights: map[ColorType]CastlingRights{
					WhitePiece: {KingSide: true, QueenSide: false},
					BlackPiece: {KingSide: true, QueenSide: false},
				},
			}
			movement := NewChessMovement(position)
			movement.Calculate()
			assert.Equal(t, test.canCastle, movement.CanCastle.KingSide)
		})
	}
}

func TestQueenSideCastlingPathBlocked(t *testing.T) {
	tests := []struct {
		name         string
		boardSetup   string
		playerToMove ColorType
		canCastle    bool
	}{
		{
			name:         "white queen-side castling clear path",
			boardSetup:   "Ke1 Ra1 ke8",
			playerToMove: WhitePiece,
			canCastle:    true,
		},
		{
			name:         "white queen-side castling blocked by piece on d1",
			boardSetup:   "Ke1 Ra1 Qd1 ke8",
			playerToMove: WhitePiece,
			canCastle:    false,
		},
		{
			name:         "white queen-side castling blocked by piece on c1",
			boardSetup:   "Ke1 Ra1 Bc1 ke8",
			playerToMove: WhitePiece,
			canCastle:    false,
		},
		{
			name:         "white queen-side castling blocked by piece on b1",
			boardSetup:   "Ke1 Ra1 Nb1 ke8",
			playerToMove: WhitePiece,
			canCastle:    false,
		},
		{
			name:         "black queen-side castling clear path",
			boardSetup:   "ke8 ra8 Ke1",
			playerToMove: BlackPiece,
			canCastle:    true,
		},
		{
			name:         "black queen-side castling blocked by piece on d8",
			boardSetup:   "ke8 ra8 qd8 Ke1",
			playerToMove: BlackPiece,
			canCastle:    false,
		},
		{
			name:         "black queen-side castling blocked by piece on c8",
			boardSetup:   "ke8 ra8 bc8 Ke1",
			playerToMove: BlackPiece,
			canCastle:    false,
		},
		{
			name:         "black queen-side castling blocked by piece on b8",
			boardSetup:   "ke8 ra8 nb8 Ke1",
			playerToMove: BlackPiece,
			canCastle:    false,
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := parseBoard(test.boardSetup)
			position := &ChessPosition{
				Board:        board,
				PlayerToMove: test.playerToMove,
				CastlingRights: map[ColorType]CastlingRights{
					WhitePiece: {KingSide: false, QueenSide: true},
					BlackPiece: {KingSide: false, QueenSide: true},
				},
			}
			movement := NewChessMovement(position)
			movement.Calculate()
			assert.Equal(t, test.canCastle, movement.CanCastle.QueenSide)
		})
	}
}


func TestValidPawnMoves(t *testing.T) {
	tests := []struct {
		name             string
		boardSetup       string
		playerToMove     ColorType
		expectedMoves    []string
		notExpectedMoves []string
	}{
		{
			name:         "white pawn no adjacent pieces - no captures in valid moves",
			boardSetup:   "Pd5 Ke1 ke8",
			playerToMove: WhitePiece,
			// Pawn at d5 can only move forward; empty diagonal squares should NOT appear
			notExpectedMoves: []string{"Pd5xe6", "Pd5xc6"},
			expectedMoves:    []string{"Pd5d6"},
		},
		{
			name:         "white pawn with opponent piece on diagonal - capture included",
			boardSetup:   "Pd5 pe6 Ke1 ke8",
			playerToMove: WhitePiece,
			// e6 has an opponent so capture is valid; c6 is empty so no capture
			expectedMoves:    []string{"Pd5d6", "Pd5xe6"},
			notExpectedMoves: []string{"Pd5xc6"},
		},
		{
			name:         "black pawn no adjacent pieces - no captures in valid moves",
			boardSetup:   "pd5 Ke1 ke8",
			playerToMove: BlackPiece,
			// Pawn at d5 can only move forward; empty diagonal squares should NOT appear
			notExpectedMoves: []string{"pd5xe4", "pd5xc4"},
			expectedMoves:    []string{"pd5d4"},
		},
		{
			name:         "black pawn with opponent piece on diagonal - capture included",
			boardSetup:   "pd5 Pe4 Ke1 ke8",
			playerToMove: BlackPiece,
			// e4 has an opponent so capture is valid; c4 is empty so no capture
			expectedMoves:    []string{"pd5d4", "pd5xe4"},
			notExpectedMoves: []string{"pd5xc4"},
		},
		{
			name:         "white pawn starting square no captures in valid moves",
			boardSetup:   "Pd2 Ke1 ke8",
			playerToMove: WhitePiece,
			// Starting square pawn can move 1 or 2 squares; no captures on empty diagonals
			expectedMoves:    []string{"Pd2d3", "Pd2d4"},
			notExpectedMoves: []string{"Pd2xe3", "Pd2xc3"},
		},
		{
			name:         "white pawn on rank 7 generates one promotion move",
			boardSetup:   "Pe7 Ke1 ka8",
			playerToMove: WhitePiece,
			expectedMoves: []string{
				"Pe7e8=",
			},
			notExpectedMoves: []string{"Pe7e8"},
		},
		{
			name:         "black pawn on rank 2 generates one promotion move",
			boardSetup:   "pe2 ka8 Kh1",
			playerToMove: BlackPiece,
			expectedMoves: []string{
				"pe2e1=",
			},
			notExpectedMoves: []string{"pe2e1"},
		},
		{
			name:         "white pawn on rank 7 captures to promotion square generates one capture-promotion move",
			boardSetup:   "Pe7 rd8 Ke1 ka8",
			playerToMove: WhitePiece,
			expectedMoves: []string{
				"Pe7xd8=",
			},
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := parseBoard(test.boardSetup)
			position := &ChessPosition{Board: board, PlayerToMove: test.playerToMove}
			movement := NewChessMovement(position)
			movement.Calculate()
			actualMoves := getMoveStrings(movement.Moves)
			for _, expected := range test.expectedMoves {
				assert.Contains(t, actualMoves, expected)
			}
			for _, notExpected := range test.notExpectedMoves {
				assert.NotContains(t, actualMoves, notExpected)
			}
		})
	}
}
