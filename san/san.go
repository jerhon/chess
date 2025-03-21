package san

import (
	"github.com/jerhon/chess/game"
	"strings"
)

type SanMove struct {
	Piece          game.PieceType
	FromFile       game.FileType
	FromRank       game.RankType
	ToFile         game.FileType
	ToRank         game.RankType
	Capture        bool
	Check          bool
	Checkmate      bool
	EnPassant      bool
	PromotionPiece game.PieceType
}

type SanCastle struct {
	CastleKingSide  bool
	CastleQueenSide bool
}

func (this SanCastle) String() string {
	if this.CastleKingSide {
		return "O-O"
	} else if this.CastleQueenSide {
		return "O-O-O"
	} else {
		return ""
	}
}

func (this SanMove) String() string {
	builder := strings.Builder{}
	if this.Piece != game.NoPiece && this.Piece != game.Pawn {
		builder.WriteString(string(this.Piece))
	}
	if this.FromFile != game.NoFile {
		builder.WriteString(string(this.FromFile))
	}
	if this.FromRank != game.NoRank {
		builder.WriteString(string(this.FromRank))
	}
	if this.Capture {
		builder.WriteString("x")
	}
	if this.ToFile != game.NoFile {
		builder.WriteString(string(this.ToFile))
	}
	if this.ToRank != game.NoRank {
		builder.WriteString(string(this.ToRank))
	}
	if this.PromotionPiece != game.NoPiece {
		builder.WriteString("=")
		builder.WriteRune(rune(this.PromotionPiece))
	}
	if this.Check {
		builder.WriteString("+")
	}
	if this.Checkmate {
		builder.WriteString("#")
	}
	if this.EnPassant {
		builder.WriteString(" e.p.")
	}

	return builder.String()
}
