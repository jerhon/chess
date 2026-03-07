package game

import "unicode"

type PieceType rune

const (
	NoPiece PieceType = iota
	Pawn    PieceType = 'P'
	Knight  PieceType = 'N'
	Bishop  PieceType = 'B'
	Rook    PieceType = 'R'
	Queen   PieceType = 'Q'
	King    PieceType = 'K'
)

type ChessPiece struct {
	Piece PieceType
	Color ColorType
}

// PrettyString returns the solid Unicode chess piece glyph for the piece.
// Both White and Black use the filled (solid) variants; the caller is
// responsible for applying foreground color to distinguish the two players.
func (s ChessPiece) PrettyString() string {
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
	default:
		return " "
	}
}

// IsValid checks if the ChessPiece is a valid option.
func (p PieceType) IsPiece() bool {
	switch p {
	case Pawn, Knight, Bishop, Rook, Queen, King:
		return true
	default:
		return false
	}
}

func ParsePiece(piece string) ChessPiece {
	if len(piece) < 1 {
		return ChessPiece{Piece: NoPiece}
	} else {
		pieceChar := rune(piece[0])
		color := WhitePiece
		if unicode.IsLower(pieceChar) {
			color = BlackPiece
		}

		piece := PieceType(unicode.ToUpper(rune(piece[0])))
		if piece.IsPiece() {
			return ChessPiece{Piece: piece, Color: color}
		} else {
			return ChessPiece{Piece: NoPiece}
		}
	}
}
