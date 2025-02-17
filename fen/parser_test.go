package fen

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestFenParser_ParseBoard(t *testing.T) {

	tests := []struct {
		name     string
		fen      string
		expected string
	}{
		{
			"standard chess position",
			"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR",
			"8 ♜♞♝♛♚♝♞♜\n7 ♟♟♟♟♟♟♟♟\n6 ********\n5 ********\n4 ********\n3 ********\n2 ♙♙♙♙♙♙♙♙\n1 ♖♘♗♕♔♗♘♖\n  abcdefgh",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			fenParser := NewFenParser(tt.fen)
			a := assert.New(t)
			board, err := fenParser.ParseBoard()
			a.Nil(err)
			ascii := board.String()
			a.Equal(tt.expected, ascii)
		})
	}
}
