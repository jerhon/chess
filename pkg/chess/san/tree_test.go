package san

import (
	"github.com/jerhon/chess/pkg/chess/game"
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
				Piece:          game.Pawn,
				ToFile:         game.FileE,
				ToRank:         game.Rank4,
				Capture:        false,
				Check:          false,
				Checkmate:      false,
				PromotionPiece: game.NoPiece,
			},
			err: nil,
		},
		{
			input: "Nf3",
			sanMove: &SanMove{
				Piece:   game.Knight,
				ToFile:  game.FileF,
				ToRank:  game.Rank3,
				Capture: false,
			},
			err: nil,
		},
		{
			input: "exd5",
			sanMove: &SanMove{
				Piece:    game.Pawn,
				ToFile:   game.FileD,
				ToRank:   game.Rank5,
				FromFile: game.FileE,
				Capture:  true,
			},
			err: nil,
		},
		{
			input: "e4+",
			sanMove: &SanMove{
				Piece:  game.Pawn,
				ToFile: game.FileE,
				ToRank: game.Rank4,
				Check:  true,
			},
			err: nil,
		},
		{
			input: "Qd8#",
			sanMove: &SanMove{
				Piece:     game.Queen,
				ToFile:    game.FileD,
				ToRank:    game.Rank8,
				Checkmate: true,
			},
			err: nil,
		},
		{
			input: "exd6 e.p.",
			sanMove: &SanMove{
				Piece:     game.Pawn,
				ToFile:    game.FileD,
				ToRank:    game.Rank6,
				FromFile:  game.FileE,
				Capture:   true,
				EnPassant: true,
			},
			err: nil,
		},
		{
			input: "e8=Q",
			sanMove: &SanMove{
				Piece:          game.Pawn,
				ToFile:         game.FileE,
				ToRank:         game.Rank8,
				PromotionPiece: game.Queen,
			},
			err: nil,
		},
		{
			input: "g1=N",
			sanMove: &SanMove{
				Piece:          game.Pawn,
				ToFile:         game.FileG,
				ToRank:         game.Rank1,
				PromotionPiece: game.Knight,
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
