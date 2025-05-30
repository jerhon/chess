package game

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func l(file FileType, rank RankType) ChessLocation {
	return ChessLocation{file, rank}
}

func p(piece PieceType, color ColorType) ChessPiece {
	return ChessPiece{piece, color}
}

func TestChessBoard_String(t *testing.T) {
	tests := []struct {
		name   string
		setup  func() *ChessBoard
		output string
	}{
		{
			name: "Empty Board",
			setup: func() *ChessBoard {
				board := NewChessBoard() // Assuming this initializes an empty game
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
				board := NewChessBoard() // Assuming this initializes the game in standard chess setup
				// Set up pawns, rooks, knights, bishops, king, and queen
				for file := FileA; file <= FileH; file++ {
					board.SetSquare(l(file, Rank2), p(Pawn, WhitePiece))
					board.SetSquare(l(file, Rank7), p(Pawn, BlackPiece))
				}
				board.SetSquare(l(FileA, Rank1), p(Rook, WhitePiece))
				board.SetSquare(l(FileH, Rank1), p(Rook, WhitePiece))
				board.SetSquare(l(FileA, Rank8), p(Rook, BlackPiece))
				board.SetSquare(l(FileH, Rank8), p(Rook, BlackPiece))
				board.SetSquare(l(FileB, Rank1), p(Knight, WhitePiece))
				board.SetSquare(l(FileG, Rank1), p(Knight, WhitePiece))
				board.SetSquare(l(FileB, Rank8), p(Knight, BlackPiece))
				board.SetSquare(l(FileG, Rank8), p(Knight, BlackPiece))
				board.SetSquare(l(FileC, Rank1), p(Bishop, WhitePiece))
				board.SetSquare(l(FileF, Rank1), p(Bishop, WhitePiece))
				board.SetSquare(l(FileC, Rank8), p(Bishop, BlackPiece))
				board.SetSquare(l(FileF, Rank8), p(Bishop, BlackPiece))
				board.SetSquare(l(FileD, Rank1), p(Queen, WhitePiece))
				board.SetSquare(l(FileE, Rank1), p(King, WhitePiece))
				board.SetSquare(l(FileD, Rank8), p(Queen, BlackPiece))
				board.SetSquare(l(FileE, Rank8), p(King, BlackPiece))
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
				board.SetSquare(l(FileC, Rank3), p(Queen, WhitePiece))
				board.SetSquare(l(FileE, Rank6), p(Knight, BlackPiece))
				board.SetSquare(l(FileH, Rank8), p(King, BlackPiece))
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

func TestChessBoard_HasPiece(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(l(FileA, Rank1), p(Pawn, WhitePiece))
	assert.True(t, board.HasPiece(l(FileA, Rank1)))
	assert.False(t, board.HasPiece(l(FileA, Rank2)))
}

func TestChessBoard_GetPiece(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(l(FileA, Rank1), p(Pawn, WhitePiece))

	piece, hasPiece := board.GetPiece(l(FileA, Rank1))
	assert.Equal(t, p(Pawn, WhitePiece), piece)
	assert.True(t, hasPiece)

	piece, hasPiece = board.GetPiece(l(FileA, Rank2))
	assert.Equal(t, ChessPiece{}, piece)
	assert.False(t, hasPiece)
}

func TestChessBoard_Clone(t *testing.T) {
	board := NewChessBoard()
	clonedBoard := board.Clone()
	assert.Equal(t, board, clonedBoard)

	// set a bunch of random pieces, and ensure they match
	board.SetSquare(l(FileA, Rank1), p(Pawn, WhitePiece))
	board.SetSquare(l(FileA, Rank2), p(Pawn, BlackPiece))
	board.SetSquare(l(FileA, Rank3), p(Pawn, WhitePiece))
	board.SetSquare(l(FileE, Rank6), p(Knight, BlackPiece))
	board.SetSquare(l(FileH, Rank8), p(King, BlackPiece))

	clonedBoard = board.Clone()
	assert.Equal(t, board, clonedBoard)
}
