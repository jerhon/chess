package chess

import (
	"iter"
	"strings"
)

type ChessPiece struct {
	Piece PieceType
	Color ColorType
}

type ChessLocation struct {
	File FileType
	Rank RankType
}

type ChessSquare struct {
	Location ChessLocation
	Piece    ChessPiece
}

type ChessBoard struct {
	squares map[string]ChessSquare
}

func (b ChessBoard) GetSquare(file FileType, rank RankType) ChessSquare {
	value, exists := b.squares[string(file)+string(rank)]
	if exists {
		return value
	} else {
		return ChessSquare{ChessLocation{file, rank}, ChessPiece{NoPiece, NoColor}}
	}
}

func (b ChessBoard) SetSquare(file FileType, rank RankType, piece ChessPiece) {
	b.squares[string(file)+string(rank)] = ChessSquare{ChessLocation{file, rank}, piece}
}

func (b ChessBoard) IterateSquares() iter.Seq[ChessSquare] {
	return func(yield func(square ChessSquare) bool) {
		for rank := Rank8; rank >= Rank1; rank-- {
			for file := FileA; file <= FileH; file++ {
				yield(b.GetSquare(file, rank))
			}
		}
		return
	}
}

func (s ChessSquare) String() string {
	if s.Piece.Piece == NoPiece {
		return string(s.Location.File) + string(s.Location.Rank)
	} else {
		return string(s.Piece.Color) + string(s.Piece.Piece) + string(s.Location.File) + string(s.Location.Rank)
	}
}

func (s ChessPiece) String() string {
	if s.Piece != NoPiece {
		if s.Color == WhitePiece {
			switch s.Piece {
			case Pawn:
				return "\u2659"
			case Knight:
				return "\u2658"
			case Bishop:
				return "\u2657"
			case Rook:
				return "\u2656"
			case Queen:
				return "\u2655"
			case King:
				return "\u2654"
			}
		} else {
			switch s.Piece {
			case Pawn:
				return "\u265F"
			case Knight:
				return "\u265E"
			case Bishop:
				return "\u265D"
			case Rook:
				return "\u265C"
			case Queen:
				return "\u265B"
			case King:
				return "\u265A"
			}
		}
	}

	return "*"
}

// / String creates the string representation of a chess board
func (b ChessBoard) String() string {
	sb := strings.Builder{}

	for square := range b.IterateSquares() {
		if square.Location.File == FileA {
			sb.WriteRune(rune(square.Location.Rank))
			sb.WriteRune(' ')
		}
		sb.WriteString(square.Piece.String())
		if square.Location.File == FileH {
			sb.WriteString("\n")
		}
	}

	sb.WriteString("  abcdefgh")

	return sb.String()
}

func (file FileType) ToIndex() int {
	return int(file) - int(FileA)
}

func (rank RankType) ToIndex() int {
	return int(rank) - int(Rank1)
}

func NewChessBoard() *ChessBoard {
	board := &ChessBoard{squares: make(map[string]ChessSquare)}
	return board
}
