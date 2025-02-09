package chess

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestChessBoard_String(t *testing.T) {
	tests := []struct {
		name   string
		setup  func() *ChessBoard
		output string
	}{
		{
			name: "Empty Board",
			setup: func() *ChessBoard {
				board := NewChessBoard() // Assuming this initializes an empty board
				return board
			},
			output: `8 ********
7 ********
6 ********
5 ********
4 ********
3 ********
2 ********
1 ********
  abcdefgh`,
		},
		{
			name: "Initial Setup",
			setup: func() *ChessBoard {
				board := NewChessBoard() // Assuming this initializes the board in standard chess setup
				// Set up pawns, rooks, knights, bishops, king, and queen
				for file := FileA; file <= FileH; file++ {
					board.SetSquare(file, Rank2, ChessPiece{Piece: Pawn, Color: WhitePiece})
					board.SetSquare(file, Rank7, ChessPiece{Piece: Pawn, Color: BlackPiece})
				}
				board.SetSquare(FileA, Rank1, ChessPiece{Piece: Rook, Color: WhitePiece})
				board.SetSquare(FileH, Rank1, ChessPiece{Piece: Rook, Color: WhitePiece})
				board.SetSquare(FileA, Rank8, ChessPiece{Piece: Rook, Color: BlackPiece})
				board.SetSquare(FileH, Rank8, ChessPiece{Piece: Rook, Color: BlackPiece})
				board.SetSquare(FileB, Rank1, ChessPiece{Piece: Knight, Color: WhitePiece})
				board.SetSquare(FileG, Rank1, ChessPiece{Piece: Knight, Color: WhitePiece})
				board.SetSquare(FileB, Rank8, ChessPiece{Piece: Knight, Color: BlackPiece})
				board.SetSquare(FileG, Rank8, ChessPiece{Piece: Knight, Color: BlackPiece})
				board.SetSquare(FileC, Rank1, ChessPiece{Piece: Bishop, Color: WhitePiece})
				board.SetSquare(FileF, Rank1, ChessPiece{Piece: Bishop, Color: WhitePiece})
				board.SetSquare(FileC, Rank8, ChessPiece{Piece: Bishop, Color: BlackPiece})
				board.SetSquare(FileF, Rank8, ChessPiece{Piece: Bishop, Color: BlackPiece})
				board.SetSquare(FileD, Rank1, ChessPiece{Piece: Queen, Color: WhitePiece})
				board.SetSquare(FileE, Rank1, ChessPiece{Piece: King, Color: WhitePiece})
				board.SetSquare(FileD, Rank8, ChessPiece{Piece: Queen, Color: BlackPiece})
				board.SetSquare(FileE, Rank8, ChessPiece{Piece: King, Color: BlackPiece})
				return board
			},
			output: `8 ♜♞♝♛♚♝♞♜
7 ♟♟♟♟♟♟♟♟
6 ********
5 ********
4 ********
3 ********
2 ♙♙♙♙♙♙♙♙
1 ♖♘♗♕♔♗♘♖
  abcdefgh`,
		},
		{
			name: "Partially Filled Board",
			setup: func() *ChessBoard {
				board := NewChessBoard()
				board.SetSquare(FileC, Rank3, ChessPiece{Piece: Queen, Color: WhitePiece})
				board.SetSquare(FileE, Rank6, ChessPiece{Piece: Knight, Color: BlackPiece})
				board.SetSquare(FileH, Rank8, ChessPiece{Piece: King, Color: BlackPiece})
				return board
			},
			output: `8 *******♚
7 ********
6 ****♞***
5 ********
4 ********
3 **♕*****
2 ********
1 ********
  abcdefgh`,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			board := tt.setup()
			assert.Equal(t, tt.output, board.String())
		})
	}
}

func TestChessPiece_String(t *testing.T) {
	tests := []struct {
		name  string
		piece ChessPiece
		want  string
	}{
		{
			name:  "White Pawn",
			piece: ChessPiece{Piece: Pawn, Color: WhitePiece},
			want:  "\u2659",
		},
		{
			name:  "White Knight",
			piece: ChessPiece{Piece: Knight, Color: WhitePiece},
			want:  "\u2658",
		},
		{
			name:  "White Bishop",
			piece: ChessPiece{Piece: Bishop, Color: WhitePiece},
			want:  "\u2657",
		},
		{
			name:  "White Rook",
			piece: ChessPiece{Piece: Rook, Color: WhitePiece},
			want:  "\u2656",
		},
		{
			name:  "White Queen",
			piece: ChessPiece{Piece: Queen, Color: WhitePiece},
			want:  "\u2655",
		},
		{
			name:  "White King",
			piece: ChessPiece{Piece: King, Color: WhitePiece},
			want:  "\u2654",
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
			want:  "*",
		},
		{
			name:  "No Piece Black",
			piece: ChessPiece{Piece: NoPiece, Color: BlackPiece},
			want:  "*",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := tt.piece.String(); got != tt.want {
				t.Errorf("ChessPiece.String() = %v, want %v", got, tt.want)
			}
		})
	}
}
