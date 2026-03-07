package game

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestMove_CastlingRightsPreserved(t *testing.T) {
	tests := []struct {
		name              string
		from              ChessLocation
		to                ChessLocation
		playerToMove      ColorType
		initialRights     map[ColorType]CastlingRights
		expectedRights    map[ColorType]CastlingRights
	}{
		{
			name:         "non-special piece move preserves all castling rights",
			from:         ChessLocation{FileD, Rank2},
			to:           ChessLocation{FileD, Rank4},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "white king move clears white castling rights",
			from:         ChessLocation{FileE, Rank1},
			to:           ChessLocation{FileF, Rank1},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: false, QueenSide: false},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "black king move clears black castling rights",
			from:         ChessLocation{FileE, Rank8},
			to:           ChessLocation{FileF, Rank8},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: false, QueenSide: false},
			},
		},
		{
			name:         "white kingside rook move clears white kingside castling right",
			from:         ChessLocation{FileH, Rank1},
			to:           ChessLocation{FileH, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: false, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "white queenside rook move clears white queenside castling right",
			from:         ChessLocation{FileA, Rank1},
			to:           ChessLocation{FileA, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: false},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "black kingside rook move clears black kingside castling right",
			from:         ChessLocation{FileH, Rank8},
			to:           ChessLocation{FileH, Rank6},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: false, QueenSide: true},
			},
		},
		{
			name:         "black queenside rook move clears black queenside castling right",
			from:         ChessLocation{FileA, Rank8},
			to:           ChessLocation{FileA, Rank6},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: false},
			},
		},
		{
			name:         "rook not on starting rank does not affect castling rights",
			from:         ChessLocation{FileH, Rank5},
			to:           ChessLocation{FileH, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := NewChessBoard()

			// Place the moving piece on the board
			var piece ChessPiece
			switch {
			case test.from == (ChessLocation{FileE, Rank1}) || test.from == (ChessLocation{FileE, Rank8}):
				piece = ChessPiece{King, test.playerToMove}
			case test.from.File == FileA || test.from.File == FileH:
				piece = ChessPiece{Rook, test.playerToMove}
			default:
				piece = ChessPiece{Pawn, test.playerToMove}
			}
			board.SetSquare(test.from, piece)

			position := &ChessPosition{
				Board:          board,
				PlayerToMove:   test.playerToMove,
				CastlingRights: test.initialRights,
			}

			result := position.Move(test.from, test.to)
			assert.Equal(t, test.expectedRights, result.CastlingRights)
		})
	}
}

// TestMove_BlackKingsideRookFix is a regression test for the bug where
// the black kingside rook check incorrectly used FileE instead of FileH.
func TestMove_BlackKingsideRookFix(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileH, Rank8}, ChessPiece{Rook, BlackPiece})

	position := &ChessPosition{
		Board:        board,
		PlayerToMove: BlackPiece,
		CastlingRights: map[ColorType]CastlingRights{
			WhitePiece: {KingSide: true, QueenSide: true},
			BlackPiece: {KingSide: true, QueenSide: true},
		},
	}

	result := position.Move(ChessLocation{FileH, Rank8}, ChessLocation{FileH, Rank6})

	// Kingside right must be cleared; queenside must remain
	assert.False(t, result.CastlingRights[BlackPiece].KingSide, "black kingside castling right should be cleared after H8 rook moves")
	assert.True(t, result.CastlingRights[BlackPiece].QueenSide, "black queenside castling right should be preserved")
	// White rights must be untouched
	assert.True(t, result.CastlingRights[WhitePiece].KingSide)
	assert.True(t, result.CastlingRights[WhitePiece].QueenSide)
}
