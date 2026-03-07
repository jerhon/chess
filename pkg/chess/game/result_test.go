package game

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestGameResult_IsDraw(t *testing.T) {
	drawResults := []GameResult{
		DrawStalemate, DrawFiftyMove, DrawInsufficientMaterial, DrawRepetition, DrawAgreement,
	}
	for _, r := range drawResults {
		assert.True(t, r.IsDraw(), "%s should be a draw", r)
	}

	nonDrawResults := []GameResult{InProgress, WhiteWins, BlackWins}
	for _, r := range nonDrawResults {
		assert.False(t, r.IsDraw(), "%s should not be a draw", r)
	}
}

func TestGameResult_IsDecided(t *testing.T) {
	assert.False(t, InProgress.IsDecided())

	decidedResults := []GameResult{
		WhiteWins, BlackWins, DrawStalemate, DrawFiftyMove,
		DrawInsufficientMaterial, DrawRepetition, DrawAgreement,
	}
	for _, r := range decidedResults {
		assert.True(t, r.IsDecided(), "%s should be decided", r)
	}
}

func TestGameResult_String(t *testing.T) {
	cases := map[GameResult]string{
		InProgress:               "In Progress",
		WhiteWins:                "White Wins",
		BlackWins:                "Black Wins",
		DrawStalemate:            "Draw (Stalemate)",
		DrawFiftyMove:            "Draw (Fifty-Move Rule)",
		DrawInsufficientMaterial: "Draw (Insufficient Material)",
		DrawRepetition:           "Draw (Threefold Repetition)",
		DrawAgreement:            "Draw (Agreement)",
	}
	for result, want := range cases {
		assert.Equal(t, want, result.String())
	}
}

func TestHasInsufficientMaterial_KvsK(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	assert.True(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_KBvsK(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileC, Rank1}, ChessPiece{Bishop, WhitePiece})
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	assert.True(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_KNvsK(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileG, Rank1}, ChessPiece{Knight, WhitePiece})
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	assert.True(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_KBvsKBSameColor(t *testing.T) {
	board := NewChessBoard()
	// c1 is a dark square (file index 2, rank index 0 → even)
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileC, Rank1}, ChessPiece{Bishop, WhitePiece}) // dark square
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	board.SetSquare(ChessLocation{FileA, Rank3}, ChessPiece{Bishop, BlackPiece}) // dark square (0+2=even)
	assert.True(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_KBvsKBDifferentColor(t *testing.T) {
	board := NewChessBoard()
	// c1 is a dark square; d1 is a light square
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileC, Rank1}, ChessPiece{Bishop, WhitePiece}) // dark square
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	board.SetSquare(ChessLocation{FileD, Rank1}, ChessPiece{Bishop, BlackPiece}) // light square (3+0=odd)
	assert.False(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_WithPawn(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{FileE, Rank2}, ChessPiece{Pawn, WhitePiece})
	board.SetSquare(ChessLocation{FileE, Rank8}, ChessPiece{King, BlackPiece})
	assert.False(t, hasInsufficientMaterial(board))
}

func TestHasInsufficientMaterial_FullBoard(t *testing.T) {
	board := NewStandardChessBoard()
	assert.False(t, hasInsufficientMaterial(board))
}
