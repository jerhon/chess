package san

import (
	game2 "github.com/jerhon/chess/pkg/chess/game"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestParseSan(t *testing.T) {
	tests := []struct {
		input           string
		sanMove         *SanMove
		sanCastlingMove *SanCastle
		err             error
	}{
		{
			input: "e4",
			sanMove: &SanMove{
				Piece:          game2.Pawn,
				ToFile:         game2.FileE,
				ToRank:         game2.Rank4,
				Capture:        false,
				Check:          false,
				Checkmate:      false,
				PromotionPiece: game2.NoPiece,
			},
			err: nil,
		},
		{
			input: "Nf3",
			sanMove: &SanMove{
				Piece:   game2.Knight,
				ToFile:  game2.FileF,
				ToRank:  game2.Rank3,
				Capture: false,
			},
			err: nil,
		},
		{
			input: "exd5",
			sanMove: &SanMove{
				Piece:    game2.Pawn,
				ToFile:   game2.FileD,
				ToRank:   game2.Rank5,
				FromFile: game2.FileE,
				Capture:  true,
			},
			err: nil,
		},
		{
			input: "e4+",
			sanMove: &SanMove{
				Piece:  game2.Pawn,
				ToFile: game2.FileE,
				ToRank: game2.Rank4,
				Check:  true,
			},
			err: nil,
		},
		{
			input: "Qd8#",
			sanMove: &SanMove{
				Piece:     game2.Queen,
				ToFile:    game2.FileD,
				ToRank:    game2.Rank8,
				Checkmate: true,
			},
			err: nil,
		},
		{
			input: "exd6 e.p.",
			sanMove: &SanMove{
				Piece:     game2.Pawn,
				ToFile:    game2.FileD,
				ToRank:    game2.Rank6,
				FromFile:  game2.FileE,
				Capture:   true,
				EnPassant: true,
			},
			err: nil,
		},
		{
			input: "e8=Q",
			sanMove: &SanMove{
				Piece:          game2.Pawn,
				ToFile:         game2.FileE,
				ToRank:         game2.Rank8,
				PromotionPiece: game2.Queen,
			},
			err: nil,
		},
		{
			input: "g1=N",
			sanMove: &SanMove{
				Piece:          game2.Pawn,
				ToFile:         game2.FileG,
				ToRank:         game2.Rank1,
				PromotionPiece: game2.Knight,
			},
			err: nil,
		},
		{
			input: "0-0",
			sanCastlingMove: &SanCastle{
				CastleKingSide: true,
			},
		},
		{
			input: "0-0-0",
			sanCastlingMove: &SanCastle{
				CastleQueenSide: true,
			},
		},
	}

	for _, test := range tests {
		t.Run(test.input, func(t *testing.T) {
			sanMove, sanCastlingMove, err := ParseSan(test.input)
			assert.Equal(t, test.err, err)
			assert.Equal(t, test.sanMove, sanMove)
			assert.Equal(t, test.sanCastlingMove, sanCastlingMove)
		})
	}
}
