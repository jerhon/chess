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

// PrettyString returns the Unicode chess piece glyph for the piece.
// White pieces use the outline/hollow variants (U+2654–U+2659) and
// Black pieces use the solid/filled variants (U+265A–U+265F), matching
// the standard Unicode chess character convention.
func (s ChessPiece) PrettyString() string {
	// Black/solid piece codepoints: ♚♛♜♝♞♟ (U+265A–U+265F).
	// White/hollow variants are exactly 6 codepoints lower: ♔♕♖♗♘♙ (U+2654–U+2659).
	var base rune
	switch s.Piece {
	case King:
		base = '\u265A'
	case Queen:
		base = '\u265B'
	case Rook:
		base = '\u265C'
	case Bishop:
		base = '\u265D'
	case Knight:
		base = '\u265E'
	case Pawn:
		base = '\u265F'
	default:
		return " "
	}
	if s.Color == WhitePiece {
		base -= 6
	}
	return string(base)
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
