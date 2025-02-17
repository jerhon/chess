package fen

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestToFenString(t *testing.T) {
	tests := []struct {
		name     string
		expected string
	}{
		{
			name:     "initial_position",
			expected: "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
		},
		{
			name:     "en_passant_position",
			expected: "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",
		},
		{
			name:     "castling_rights_lost",
			expected: "rnbqkbnr/pppppppp/8/8/3Pp3/8/PPP2PPP/RNBQKBNR w KQ - 0 2",
		},
		{
			name:     "no_castling_and_en_passant",
			expected: "8/8/8/8/8/8/k7/K7 b - - 50 99",
		},
		{
			name:     "pawn_promotion",
			expected: "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK1NR w Qkq - 0 1",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			a := assert.New(t)
			board, err := ParseFen(tt.expected)
			a.Nil(err)
			result := ToFenString(&board)
			a.Equal(tt.expected, result)
			if result != tt.expected {
				t.Errorf("expected %q but got %q", tt.expected, result)
			}
		})
	}
}
