package board

import (
	"iter"
	"strings"
)

type ColorType rune

const (
	NoColor    ColorType = iota
	WhitePiece ColorType = 'w'
	BlackPiece ColorType = 'b'
)

type ChessBoard struct {
	squares map[string]ChessSquare
}

func (b ChessBoard) GetSquare(location ChessLocation) ChessSquare {
	value, exists := b.squares[location.String()]
	if exists {
		return value
	} else {
		return ChessSquare{location, ChessPiece{NoPiece, NoColor}}
	}
}

func (b ChessBoard) SetSquare(location ChessLocation, piece ChessPiece) {
	b.squares[location.String()] = ChessSquare{location, piece}
}

func (b ChessBoard) IterateSquares() iter.Seq[ChessSquare] {
	return func(yield func(square ChessSquare) bool) {
		for rank := Rank8; rank >= Rank1; rank-- {
			for file := FileA; file <= FileH; file++ {
				yield(b.GetSquare(ChessLocation{file, rank}))
			}
		}
		return
	}
}

// / String creates the string representation of a chess board
func (b ChessBoard) String() string {
	sb := strings.Builder{}

	for square := range b.IterateSquares() {
		if square.Location.File == FileA {
			sb.WriteRune(rune(square.Location.Rank))
			sb.WriteRune(' ')
		}

		if square.Piece.Piece == NoPiece {
			sb.WriteRune('*')
		} else {
			sb.WriteString(square.Piece.String())
		}

		if square.Location.File == FileH {
			sb.WriteString("\n")
		}
	}

	sb.WriteString("  abcdefgh")

	return sb.String()
}

func NewChessBoard() *ChessBoard {
	board := &ChessBoard{squares: make(map[string]ChessSquare)}
	return board
}

func (board ChessBoard) HasPiece(location ChessLocation) bool {
	return board.squares[location.String()].Piece.Piece != NoPiece
}

func (board ChessBoard) GetPiece(location ChessLocation) (ChessPiece, bool) {
	return board.squares[location.String()].Piece, board.squares[location.String()].Piece.Piece != NoPiece
}

func (board ChessBoard) Clone() *ChessBoard {
	newBoard := NewChessBoard()
	for location, square := range board.squares {
		newBoard.squares[location] = square
	}
	return newBoard
}

func (s ChessSquare) IsEmpty() bool {
	return s.Piece.Piece == NoPiece
}
