package san

import (
	"github.com/jerhon/chess/pkg/chess/game"
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

func (s SanCastle) String() string {
	if s.CastleKingSide {
		return "O-O"
	} else if s.CastleQueenSide {
		return "O-O-O"
	} else {
		return ""
	}
}

func (s SanMove) String() string {
	builder := strings.Builder{}
	if s.Piece != game.NoPiece && s.Piece != game.Pawn {
		builder.WriteString(string(s.Piece))
	}
	if s.FromFile != game.NoFile {
		builder.WriteString(string(s.FromFile))
	}
	if s.FromRank != game.NoRank {
		builder.WriteString(string(s.FromRank))
	}
	if s.Capture {
		builder.WriteString("x")
	}
	if s.ToFile != game.NoFile {
		builder.WriteString(string(s.ToFile))
	}
	if s.ToRank != game.NoRank {
		builder.WriteString(string(s.ToRank))
	}
	if s.PromotionPiece != game.NoPiece {
		builder.WriteString("=")
		builder.WriteRune(rune(s.PromotionPiece))
	}
	if s.Check {
		builder.WriteString("+")
	}
	if s.Checkmate {
		builder.WriteString("#")
	}
	if s.EnPassant {
		builder.WriteString(" e.p.")
	}

	return builder.String()
}
