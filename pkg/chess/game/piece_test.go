package game

import "testing"

func TestChessPiece_String(t *testing.T) {
	tests := []struct {
		name  string
		piece ChessPiece
		want  string
	}{
		{
			name:  "White Pawn",
			piece: ChessPiece{Piece: Pawn, Color: WhitePiece},
			want:  "\u265F",
		},
		{
			name:  "White Knight",
			piece: ChessPiece{Piece: Knight, Color: WhitePiece},
			want:  "\u265E",
		},
		{
			name:  "White Bishop",
			piece: ChessPiece{Piece: Bishop, Color: WhitePiece},
			want:  "\u265D",
		},
		{
			name:  "White Rook",
			piece: ChessPiece{Piece: Rook, Color: WhitePiece},
			want:  "\u265C",
		},
		{
			name:  "White Queen",
			piece: ChessPiece{Piece: Queen, Color: WhitePiece},
			want:  "\u265B",
		},
		{
			name:  "White King",
			piece: ChessPiece{Piece: King, Color: WhitePiece},
			want:  "\u265A",
		},
		{
			name:  "Black Pawn",
			piece: ChessPiece{Piece: Pawn, Color: BlackPiece},
			want:  "\u265F",
		},
		{
			name:  "Black Knight",
			piece: ChessPiece{Piece: Knight, Color: BlackPiece},
			want:  "\u265E",
		},
		{
			name:  "Black Bishop",
			piece: ChessPiece{Piece: Bishop, Color: BlackPiece},
			want:  "\u265D",
		},
		{
			name:  "Black Rook",
			piece: ChessPiece{Piece: Rook, Color: BlackPiece},
			want:  "\u265C",
		},
		{
			name:  "Black Queen",
			piece: ChessPiece{Piece: Queen, Color: BlackPiece},
			want:  "\u265B",
		},
		{
			name:  "Black King",
			piece: ChessPiece{Piece: King, Color: BlackPiece},
			want:  "\u265A",
		},
		{
			name:  "No Piece",
			piece: ChessPiece{Piece: NoPiece, Color: WhitePiece},
			want:  " ",
		},
		{
			name:  "No Piece Black",
			piece: ChessPiece{Piece: NoPiece, Color: BlackPiece},
			want:  " ",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := tt.piece.PrettyString(); got != tt.want {
				t.Errorf("ChessPiece.String() = %v, want %v", got, tt.want)
			}
		})
	}
}
