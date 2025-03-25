package game

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
			want: "Pa1",
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
			want: "pa8",
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
			want: "Ke1",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := tt.s.String()
			assert.Equal(t, tt.want, got)
		})
	}
}

func TestParseSquare(t *testing.T) {
	tests := []struct {
		s    string
		want ChessSquare
	}{
		{
			s: "Pa1",
			want: ChessSquare{
				Location: ChessLocation{
					File: FileA,
					Rank: Rank1,
				},
				Piece: ChessPiece{
					Piece: Pawn,
					Color: WhitePiece,
				},
			},
		},
		{
			s: "pa8",
			want: ChessSquare{
				Location: ChessLocation{
					File: FileA,
					Rank: Rank8,
				},
				Piece: ChessPiece{
					Piece: Pawn,
					Color: BlackPiece,
				},
			},
		},
		{
			s: "Ke1",
			want: ChessSquare{
				Location: ChessLocation{
					File: FileE,
					Rank: Rank1,
				},
				Piece: ChessPiece{
					Piece: King,
					Color: WhitePiece,
				},
			},
		},
	}

	for _, tt := range tests {
		t.Run(tt.s, func(t *testing.T) {
			s := ParseSquare(tt.s)
			assert.Equal(t, tt.want, s)
		})
	}
}
