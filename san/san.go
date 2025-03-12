package san

import (
	"github.com/jerhon/chess/board"
	"strings"
)

type San struct {
	CastleKingSide  bool
	CastleQueenSide bool
	Piece           board.PieceType
	FromFile        board.FileType
	FromRank        board.RankType
	ToFile          board.FileType
	ToRank          board.RankType
	Capture         bool
	Check           bool
	Checkmate       bool
	EnPassant       bool
	PromotionPiece  board.PieceType
}

func (this San) String() string {
	if this.CastleKingSide {
		return "O-O"
	} else if this.CastleQueenSide {
		return "O-O-O"
	}

	builder := strings.Builder{}
	if this.Piece != board.NoPiece && this.Piece != board.Pawn {
		builder.WriteString(string(this.Piece))
	}
	if this.FromFile != board.NoFile {
		builder.WriteString(string(this.FromFile))
	}
	if this.FromRank != board.NoRank {
		builder.WriteString(string(this.FromRank))
	}
	if this.Capture {
		builder.WriteString("x")
	}
	if this.ToFile != board.NoFile {
		builder.WriteString(string(this.ToFile))
	}
	if this.ToRank != board.NoRank {
		builder.WriteString(string(this.ToRank))
	}
	if this.Check {
		builder.WriteString("+")
	}
	if this.Checkmate {
		builder.WriteString("#")
	}
	if this.PromotionPiece != board.NoPiece {
		builder.WriteString("=")
		builder.WriteRune(rune(this.PromotionPiece))
	}
	if this.EnPassant {
		builder.WriteString(" e.p.")
	}

	return builder.String()
}
