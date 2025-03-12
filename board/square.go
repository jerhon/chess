package board

type ChessSquare struct {
	Location ChessLocation
	Piece    ChessPiece
}

func (s ChessSquare) String() string {
	if s.Piece.Piece == NoPiece {
		return string(s.Location.File) + string(s.Location.Rank)
	} else {
		return string(s.Piece.Color) + string(s.Piece.Piece) + string(s.Location.File) + string(s.Location.Rank)
	}
}
