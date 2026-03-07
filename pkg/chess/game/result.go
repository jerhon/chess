package game

// GameResult represents the outcome of a chess game.
type GameResult int

const (
	// InProgress means the game is still ongoing.
	InProgress GameResult = iota
	// WhiteWins means White has won (e.g., by checkmate).
	WhiteWins
	// BlackWins means Black has won (e.g., by checkmate).
	BlackWins
	// DrawStalemate means the game ended in a draw by stalemate.
	DrawStalemate
	// DrawFiftyMove means the game ended in a draw by the fifty-move rule.
	DrawFiftyMove
	// DrawInsufficientMaterial means the game ended in a draw due to insufficient material.
	DrawInsufficientMaterial
	// DrawRepetition means the game ended in a draw by threefold repetition.
	DrawRepetition
	// DrawAgreement means the game ended in a draw by mutual agreement.
	DrawAgreement
)

// IsDraw returns true if the result represents any kind of draw.
func (r GameResult) IsDraw() bool {
	return r == DrawStalemate || r == DrawFiftyMove || r == DrawInsufficientMaterial ||
		r == DrawRepetition || r == DrawAgreement
}

// IsDecided returns true if the game has ended (i.e., it is not InProgress).
func (r GameResult) IsDecided() bool {
	return r != InProgress
}

// String returns a human-readable description of the game result.
func (r GameResult) String() string {
	switch r {
	case InProgress:
		return "In Progress"
	case WhiteWins:
		return "White Wins"
	case BlackWins:
		return "Black Wins"
	case DrawStalemate:
		return "Draw (Stalemate)"
	case DrawFiftyMove:
		return "Draw (Fifty-Move Rule)"
	case DrawInsufficientMaterial:
		return "Draw (Insufficient Material)"
	case DrawRepetition:
		return "Draw (Threefold Repetition)"
	case DrawAgreement:
		return "Draw (Agreement)"
	default:
		return "Unknown"
	}
}

// hasInsufficientMaterial returns true when neither side has enough material to
// force checkmate. The recognised cases are:
//   - K vs K
//   - K+B vs K or K+N vs K (one minor piece total)
//   - K+B vs K+B where both bishops travel on the same square colour
func hasInsufficientMaterial(board *ChessBoard) bool {
	var whiteKnights, whiteBishops, whiteOther int
	var blackKnights, blackBishops, blackOther int
	var whiteBishopColor, blackBishopColor int

	for square := range board.IterateSquares() {
		p := square.Piece
		if p.Piece == NoPiece || p.Piece == King {
			continue
		}
		squareColor := (square.Location.File.ToIndex() + square.Location.Rank.ToIndex()) % 2
		switch {
		case p.Color == WhitePiece && p.Piece == Knight:
			whiteKnights++
		case p.Color == WhitePiece && p.Piece == Bishop:
			whiteBishops++
			whiteBishopColor = squareColor
		case p.Color == BlackPiece && p.Piece == Knight:
			blackKnights++
		case p.Color == BlackPiece && p.Piece == Bishop:
			blackBishops++
			blackBishopColor = squareColor
		default:
			// Pawn, Rook, or Queen — sufficient mating material
			if p.Color == WhitePiece {
				whiteOther++
			} else {
				blackOther++
			}
		}
	}

	if whiteOther > 0 || blackOther > 0 {
		return false
	}

	total := whiteKnights + whiteBishops + blackKnights + blackBishops

	// K vs K
	if total == 0 {
		return true
	}

	// K+minor vs K
	if total == 1 {
		return true
	}

	// K+B vs K+B with bishops on the same square colour
	if whiteKnights == 0 && whiteBishops == 1 && blackKnights == 0 && blackBishops == 1 {
		return whiteBishopColor == blackBishopColor
	}

	return false
}
