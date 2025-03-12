package board

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestChessSquare_String(t *testing.T) {

	tests := []struct {
		name string
		s    ChessSquare
		want string
	}{
		{
			name: "White Pawn",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileA,
					Rank: Rank1,
				},
				Piece: ChessPiece{
					Piece: Pawn,
					Color: WhitePiece,
				},
			},
			want: "wPa1",
		},
		{
			name: "Black Pawn",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileA,
					Rank: Rank8,
				},
				Piece: ChessPiece{
					Piece: Pawn,
					Color: BlackPiece,
				},
			},
			want: "bPa8",
		},
		{
			name: "White King",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileE,
					Rank: Rank1,
				},
				Piece: ChessPiece{
					Piece: King,
					Color: WhitePiece,
				},
			},
			want: "wKe1",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := tt.s.String()
			assert.Equal(t, tt.want, got)
		})
	}
}
