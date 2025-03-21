package san

import (
	"github.com/jerhon/chess/game"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestSan_String(t *testing.T) {
	tests := []struct {
		San      SanMove
		Expected string
	}{
		{
			San: SanMove{
				Piece:          game.Bishop,
				FromFile:       game.FileA,
				FromRank:       game.Rank1,
				ToFile:         game.FileH,
				ToRank:         game.Rank8,
				Capture:        true,
				Check:          true,
				Checkmate:      true,
				EnPassant:      true,
				PromotionPiece: game.Queen,
			},
			Expected: "Ba1xh8=Q+# e.p.",
		},
	}

	for _, tt := range tests {
		t.Run(tt.Expected, func(t *testing.T) {
			assert.Equal(t, tt.Expected, tt.San.String())
		})
	}
}
