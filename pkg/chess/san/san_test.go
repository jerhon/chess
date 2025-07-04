package san

import (
	game2 "github.com/jerhon/chess/pkg/chess/game"
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
				Piece:          game2.Bishop,
				FromFile:       game2.FileA,
				FromRank:       game2.Rank1,
				ToFile:         game2.FileH,
				ToRank:         game2.Rank8,
				Capture:        true,
				Check:          true,
				Checkmate:      true,
				EnPassant:      true,
				PromotionPiece: game2.Queen,
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
