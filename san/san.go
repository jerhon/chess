package san

import "strings"

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

type FileType rune

const (
	NoFile FileType = iota
	FileA  FileType = 'a'
	FileB  FileType = 'b'
	FileC  FileType = 'c'
	FileD  FileType = 'd'
	FileE  FileType = 'e'
	FileF  FileType = 'f'
	FileG  FileType = 'g'
	FileH  FileType = 'h'
)

type RankType rune

const (
	NoRank RankType = iota
	Rank1  RankType = '1'
	Rank2  RankType = '2'
	Rank3  RankType = '3'
	Rank4  RankType = '4'
	Rank5  RankType = '5'
	Rank6  RankType = '6'
	Rank7  RankType = '7'
	Rank8  RankType = '8'
)

type San struct {
	CastleKingSide  bool
	CastleQueenSide bool
	Piece           PieceType
	FromFile        FileType
	FromRank        RankType
	ToFile          FileType
	ToRank          RankType
	Capture         bool
	Check           bool
	Checkmate       bool
	EnPassant       bool
	PromotionPiece  PieceType
}

func (this San) String() string {
	if this.CastleKingSide {
		return "O-O"
	} else if this.CastleQueenSide {
		return "O-O-O"
	}

	builder := strings.Builder{}
	if this.Piece != NoPiece && this.Piece != Pawn {
		builder.WriteString(string(this.Piece))
	}
	if this.FromFile != NoFile {
		builder.WriteString(string(this.FromFile))
	}
	if this.FromRank != NoRank {
		builder.WriteString(string(this.FromRank))
	}
	if this.Capture {
		builder.WriteString("x")
	}
	if this.ToFile != NoFile {
		builder.WriteString(string(this.ToFile))
	}
	if this.ToRank != NoRank {
		builder.WriteString(string(this.ToRank))
	}
	if this.Check {
		builder.WriteString("+")
	}
	if this.Checkmate {
		builder.WriteString("#")
	}
	if this.PromotionPiece != NoPiece {
		builder.WriteString("=")
		builder.WriteRune(rune(this.PromotionPiece))
	}
	if this.EnPassant {
		builder.WriteString(" e.p.")
	}

	return builder.String()
}
