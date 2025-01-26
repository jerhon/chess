package chess

import (
	"github.com/jerhon/chess/san"
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestBitboardState(t *testing.T) {
	tests := []struct {
		description string
		piece       san.PieceType
		file        san.FileType
		rank        san.RankType
		expected    string
	}{
		{"Ra8",
			san.Rook,
			san.FileA,
			san.Rank8,
			"Ra8\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000000 00000001 00000001\n" +
				"00000001 11111111 11111111\n"},
	}

	for _, test := range tests {
		t.Run(test.description, func(t *testing.T) {
			piece := ChessPiece{Piece: test.piece}
			bitboard := NewBitboard(piece)
			bitboard.Move(test.file, test.rank)
			actualOutput := bitboard.String()

			assert.Equal(t, test.expected, actualOutput)
		})
	}
}
