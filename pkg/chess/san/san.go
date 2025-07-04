package san

import (
	game2 "github.com/jerhon/chess/pkg/chess/game"
	"strings"
)

type SanMove struct {
	Piece          game2.PieceType
	FromFile       game2.FileType
	FromRank       game2.RankType
	ToFile         game2.FileType
	ToRank         game2.RankType
	Capture        bool
	Check          bool
	Checkmate      bool
	EnPassant      bool
	PromotionPiece game2.PieceType
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
	if this.Piece != game2.NoPiece && this.Piece != game2.Pawn {
		builder.WriteString(string(this.Piece))
	}
	if this.FromFile != game2.NoFile {
		builder.WriteString(string(this.FromFile))
	}
	if this.FromRank != game2.NoRank {
		builder.WriteString(string(this.FromRank))
	}
	if this.Capture {
		builder.WriteString("x")
	}
	if this.ToFile != game2.NoFile {
		builder.WriteString(string(this.ToFile))
	}
	if this.ToRank != game2.NoRank {
		builder.WriteString(string(this.ToRank))
	}
	if this.PromotionPiece != game2.NoPiece {
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
