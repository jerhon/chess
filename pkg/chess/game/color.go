package game

type ColorType rune

const (
	NoColor    ColorType = iota
	WhitePiece ColorType = 'w'
	BlackPiece ColorType = 'b'
)

var AllColors = []ColorType{WhitePiece, BlackPiece}

func (color ColorType) OppositeColor() ColorType {
	switch color {
	case WhitePiece:
		return BlackPiece
	case BlackPiece:
		return WhitePiece
	default:
		return NoColor
	}
}
