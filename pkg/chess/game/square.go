package game

import (
	"strings"
)

type ChessSquare struct {
	Location ChessLocation
	Piece    ChessPiece
}

func (s ChessSquare) String() string {
	if s.Piece.Piece == NoPiece {
		return string(s.Location.File) + string(s.Location.Rank)
	} else {
		pieceString := string(s.Piece.Piece)
		if s.Piece.Color == WhitePiece {
			pieceString = strings.ToUpper(pieceString)
		} else {
			pieceString = strings.ToLower(pieceString)
		}
		return pieceString + string(s.Location.File) + string(s.Location.Rank)
	}
}

func (s ChessSquare) IsEmpty() bool {
	return s.Piece.Piece == NoPiece
}

func ParseSquare(square string) ChessSquare {
	piece := ParsePiece(square)
	location := ParseChessLocation(square[1:])

	return ChessSquare{Piece: piece, Location: location}
}
