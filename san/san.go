package san

import (
	"github.com/jerhon/chess"
	"strings"
)

type San struct {
	CastleKingSide  bool
	CastleQueenSide bool
	Piece           chess.PieceType
	FromFile        chess.FileType
	FromRank        chess.RankType
	ToFile          chess.FileType
	ToRank          chess.RankType
	Capture         bool
	Check           bool
	Checkmate       bool
	EnPassant       bool
	PromotionPiece  chess.PieceType
}

func (this San) String() string {
	if this.CastleKingSide {
		return "O-O"
	} else if this.CastleQueenSide {
		return "O-O-O"
	}

	builder := strings.Builder{}
	if this.Piece != chess.NoPiece && this.Piece != chess.Pawn {
		builder.WriteString(string(this.Piece))
	}
	if this.FromFile != chess.NoFile {
		builder.WriteString(string(this.FromFile))
	}
	if this.FromRank != chess.NoRank {
		builder.WriteString(string(this.FromRank))
	}
	if this.Capture {
		builder.WriteString("x")
	}
	if this.ToFile != chess.NoFile {
		builder.WriteString(string(this.ToFile))
	}
	if this.ToRank != chess.NoRank {
		builder.WriteString(string(this.ToRank))
	}
	if this.Check {
		builder.WriteString("+")
	}
	if this.Checkmate {
		builder.WriteString("#")
	}
	if this.PromotionPiece != chess.NoPiece {
		builder.WriteString("=")
		builder.WriteRune(rune(this.PromotionPiece))
	}
	if this.EnPassant {
		builder.WriteString(" e.p.")
	}

	return builder.String()
}
