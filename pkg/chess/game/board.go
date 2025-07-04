package game

import (
	"iter"
	"strings"
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

func (b ChessBoard) ClearSquare(location ChessLocation) {
	delete(b.squares, location.String())
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

// / String creates the string representation of a chess game
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
			sb.WriteString(square.Piece.PrettyString())
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

// NewStandardChessBoard returns a board in a standard setup
func NewStandardChessBoard() *ChessBoard {
	board := NewChessBoard()

	for file := FileA; file <= FileH; file++ {
		board.SetSquare(ChessLocation{File: file, Rank: Rank2}, ChessPiece{Pawn, WhitePiece})
		board.SetSquare(ChessLocation{File: file, Rank: Rank7}, ChessPiece{Pawn, BlackPiece})
	}

	board.SetSquare(ChessLocation{File: FileA, Rank: Rank1}, ChessPiece{Rook, WhitePiece})
	board.SetSquare(ChessLocation{File: FileH, Rank: Rank1}, ChessPiece{Rook, WhitePiece})
	board.SetSquare(ChessLocation{File: FileA, Rank: Rank8}, ChessPiece{Rook, BlackPiece})
	board.SetSquare(ChessLocation{File: FileH, Rank: Rank8}, ChessPiece{Rook, BlackPiece})
	board.SetSquare(ChessLocation{File: FileB, Rank: Rank1}, ChessPiece{Knight, WhitePiece})
	board.SetSquare(ChessLocation{File: FileG, Rank: Rank1}, ChessPiece{Knight, WhitePiece})
	board.SetSquare(ChessLocation{File: FileB, Rank: Rank8}, ChessPiece{Knight, BlackPiece})
	board.SetSquare(ChessLocation{File: FileG, Rank: Rank8}, ChessPiece{Knight, BlackPiece})
	board.SetSquare(ChessLocation{File: FileC, Rank: Rank1}, ChessPiece{Bishop, WhitePiece})
	board.SetSquare(ChessLocation{File: FileF, Rank: Rank1}, ChessPiece{Bishop, WhitePiece})
	board.SetSquare(ChessLocation{File: FileC, Rank: Rank8}, ChessPiece{Bishop, BlackPiece})
	board.SetSquare(ChessLocation{File: FileF, Rank: Rank8}, ChessPiece{Bishop, BlackPiece})
	board.SetSquare(ChessLocation{File: FileD, Rank: Rank1}, ChessPiece{Queen, WhitePiece})
	board.SetSquare(ChessLocation{File: FileE, Rank: Rank1}, ChessPiece{King, WhitePiece})
	board.SetSquare(ChessLocation{File: FileD, Rank: Rank8}, ChessPiece{Queen, BlackPiece})
	board.SetSquare(ChessLocation{File: FileE, Rank: Rank8}, ChessPiece{King, BlackPiece})

	return board
}
