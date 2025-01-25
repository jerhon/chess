package san

import (
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
				Piece:          Pawn,
				ToFile:         FileE,
				ToRank:         Rank4,
				Capture:        false,
				Check:          false,
				Checkmate:      false,
				PromotionPiece: NoPiece,
			},
			err: nil,
		},
		{
			input: "Nf3",
			expected: &San{
				Piece:   Knight,
				ToFile:  FileF,
				ToRank:  Rank3,
				Capture: false,
			},
			err: nil,
		},
		{
			input: "exd5",
			expected: &San{
				Piece:    Pawn,
				ToFile:   FileD,
				ToRank:   Rank5,
				FromFile: FileE,
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
				Piece:  Pawn,
				ToFile: FileE,
				ToRank: Rank4,
				Check:  true,
			},
			err: nil,
		},
		{
			input: "Qd8#",
			expected: &San{
				Piece:     Queen,
				ToFile:    FileD,
				ToRank:    Rank8,
				Checkmate: true,
			},
			err: nil,
		},
		{
			input: "exd6 e.p.",
			expected: &San{
				Piece:     Pawn,
				ToFile:    FileD,
				ToRank:    Rank6,
				FromFile:  FileE,
				Capture:   true,
				EnPassant: true,
			},
			err: nil,
		},
		{
			input: "e8=Q",
			expected: &San{
				Piece:          Pawn,
				ToFile:         FileE,
				ToRank:         Rank8,
				PromotionPiece: Queen,
			},
			err: nil,
		},
		{
			input: "g1=N",
			expected: &San{
				Piece:          Pawn,
				ToFile:         FileG,
				ToRank:         Rank1,
				PromotionPiece: Knight,
			},
			err: nil,
		},
	}

	for _, test := range tests {
		t.Run(test.input, func(t *testing.T) {
			result, err := ParseSan(test.input)
			assert.Equal(t, test.err, err)
			assert.Equal(t, test.expected, result)
		})
	}
}
