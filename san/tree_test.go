package san

import (
	"github.com/jerhon/chess"
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
				Piece:          chess.Pawn,
				ToFile:         chess.FileE,
				ToRank:         chess.Rank4,
				Capture:        false,
				Check:          false,
				Checkmate:      false,
				PromotionPiece: chess.NoPiece,
			},
			err: nil,
		},
		{
			input: "Nf3",
			expected: &San{
				Piece:   chess.Knight,
				ToFile:  chess.FileF,
				ToRank:  chess.Rank3,
				Capture: false,
			},
			err: nil,
		},
		{
			input: "exd5",
			expected: &San{
				Piece:    chess.Pawn,
				ToFile:   chess.FileD,
				ToRank:   chess.Rank5,
				FromFile: chess.FileE,
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
				Piece:  chess.Pawn,
				ToFile: chess.FileE,
				ToRank: chess.Rank4,
				Check:  true,
			},
			err: nil,
		},
		{
			input: "Qd8#",
			expected: &San{
				Piece:     chess.Queen,
				ToFile:    chess.FileD,
				ToRank:    chess.Rank8,
				Checkmate: true,
			},
			err: nil,
		},
		{
			input: "exd6 e.p.",
			expected: &San{
				Piece:     chess.Pawn,
				ToFile:    chess.FileD,
				ToRank:    chess.Rank6,
				FromFile:  chess.FileE,
				Capture:   true,
				EnPassant: true,
			},
			err: nil,
		},
		{
			input: "e8=Q",
			expected: &San{
				Piece:          chess.Pawn,
				ToFile:         chess.FileE,
				ToRank:         chess.Rank8,
				PromotionPiece: chess.Queen,
			},
			err: nil,
		},
		{
			input: "g1=N",
			expected: &San{
				Piece:          chess.Pawn,
				ToFile:         chess.FileG,
				ToRank:         chess.Rank1,
				PromotionPiece: chess.Knight,
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
