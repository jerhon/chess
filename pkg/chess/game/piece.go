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

// PrettyString returns the letter representation of a chess piece.
// White pieces use uppercase letters (K, Q, R, B, N, P) and
// Black pieces use lowercase letters (k, q, r, b, n, p).
func (s ChessPiece) PrettyString() string {
	var letter rune
	switch s.Piece {
	case King:
		letter = 'K'
	case Queen:
		letter = 'Q'
	case Rook:
		letter = 'R'
	case Bishop:
		letter = 'B'
	case Knight:
		letter = 'N'
	case Pawn:
		letter = 'P'
	default:
		return " "
	}
	if s.Color == BlackPiece {
		letter = unicode.ToLower(letter)
	}
	return string(letter)
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
