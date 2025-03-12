package board

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
			case NoPiece:
				return " "
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
			case NoPiece:
				return " "
			}
		}
	}

	return " "
}
