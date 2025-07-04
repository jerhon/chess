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
		{
			name: "Empty Square",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileD,
					Rank: Rank4,
				},
				Piece: ChessPiece{
					Piece: NoPiece,
					Color: NoColor,
				},
			},
			want: "d4",
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
		{
			s: "Qd5",
			want: ChessSquare{
				Location: ChessLocation{
					File: FileD,
					Rank: Rank5,
				},
				Piece: ChessPiece{
					Piece: Queen,
					Color: WhitePiece,
				},
			},
		},
		{
			s: "rb3",
			want: ChessSquare{
				Location: ChessLocation{
					File: FileB,
					Rank: Rank3,
				},
				Piece: ChessPiece{
					Piece: Rook,
					Color: BlackPiece,
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

func TestChessSquare_IsEmpty(t *testing.T) {
	tests := []struct {
		name string
		s    ChessSquare
		want bool
	}{
		{
			name: "Empty square",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileC,
					Rank: Rank3,
				},
				Piece: ChessPiece{
					Piece: NoPiece,
					Color: NoColor,
				},
			},
			want: true,
		},
		{
			name: "White Pawn - not empty",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileA,
					Rank: Rank2,
				},
				Piece: ChessPiece{
					Piece: Pawn,
					Color: WhitePiece,
				},
			},
			want: false,
		},
		{
			name: "Black Queen - not empty",
			s: ChessSquare{
				Location: ChessLocation{
					File: FileD,
					Rank: Rank8,
				},
				Piece: ChessPiece{
					Piece: Queen,
					Color: BlackPiece,
				},
			},
			want: false,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			got := tt.s.IsEmpty()
			assert.Equal(t, tt.want, got)
		})
	}
}
