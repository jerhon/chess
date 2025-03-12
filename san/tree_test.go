package san

import (
	"github.com/jerhon/chess/board"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestParseSan(t *testing.T) {
	tests := []struct {
		input    string
		expected *San
		err      error
	}{
		{
			input: "e4",
			expected: &San{
				Piece:          board.Pawn,
				ToFile:         board.FileE,
				ToRank:         board.Rank4,
				Capture:        false,
				Check:          false,
				Checkmate:      false,
				PromotionPiece: board.NoPiece,
			},
			err: nil,
		},
		{
			input: "Nf3",
			expected: &San{
				Piece:   board.Knight,
				ToFile:  board.FileF,
				ToRank:  board.Rank3,
				Capture: false,
			},
			err: nil,
		},
		{
			input: "exd5",
			expected: &San{
				Piece:    board.Pawn,
				ToFile:   board.FileD,
				ToRank:   board.Rank5,
				FromFile: board.FileE,
				Capture:  true,
			},
			err: nil,
		},
		{
			input: "O-O",
			expected: &San{
				CastleKingSide: true,
			},
			err: nil,
		},
		{
			input: "O-O-O",
			expected: &San{
				CastleQueenSide: true,
			},
			err: nil,
		},
		{
			input: "e4+",
			expected: &San{
				Piece:  board.Pawn,
				ToFile: board.FileE,
				ToRank: board.Rank4,
				Check:  true,
			},
			err: nil,
		},
		{
			input: "Qd8#",
			expected: &San{
				Piece:     board.Queen,
				ToFile:    board.FileD,
				ToRank:    board.Rank8,
				Checkmate: true,
			},
			err: nil,
		},
		{
			input: "exd6 e.p.",
			expected: &San{
				Piece:     board.Pawn,
				ToFile:    board.FileD,
				ToRank:    board.Rank6,
				FromFile:  board.FileE,
				Capture:   true,
				EnPassant: true,
			},
			err: nil,
		},
		{
			input: "e8=Q",
			expected: &San{
				Piece:          board.Pawn,
				ToFile:         board.FileE,
				ToRank:         board.Rank8,
				PromotionPiece: board.Queen,
			},
			err: nil,
		},
		{
			input: "g1=N",
			expected: &San{
				Piece:          board.Pawn,
				ToFile:         board.FileG,
				ToRank:         board.Rank1,
				PromotionPiece: board.Knight,
			},
			err: nil,
		},
	}

	for _, test := range tests {
		t.Run(test.input, func(t *testing.T) {
			result, err := ParseSan(test.input)
			assert.Equal(t, test.err, err)
			assert.Equal(t, test.expected, result)
			assert.Equal(t, test.input, test.expected.String())
		})
	}
}
