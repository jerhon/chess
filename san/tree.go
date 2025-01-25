package san

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

func ParseSan(san string) (*San, error) {
	tokens, err := ParseSanTokens(san)
	if err != nil {
		return nil, err
	}

	piece := Pawn
	fromFile := NoFile
	fromRank := NoRank
	toFile := NoFile
	toRank := NoRank
	capture := false
	enPassant := false
	check := false
	checkmate := false
	promotionPiece := NoPiece

	// Special case for castling
	if san == "O-O" {
		return &San{
			CastleKingSide: true,
		}, nil
	} else if san == "O-O-O" {
		return &San{
			CastleQueenSide: true,
		}, nil
	}

	idx := 0
	if idx < len(tokens) && tokens[idx].Type == PieceToken {
		piece = PieceType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		toFile = FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		toRank = RankType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == CaptureToken {
		capture = true
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		fromFile = toFile
		toFile = FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		fromRank = toRank
		toRank = RankType(tokens[idx].Value[0])
		idx++
	}

	for ; idx < len(tokens); idx++ {
		if tokens[idx].Type == EnPassantToken {
			enPassant = true
		}

		if tokens[idx].Type == CheckToken {
			check = true
		}

		if tokens[idx].Type == CheckmateToken {
			checkmate = true
		}

		if tokens[idx].Type == PieceToken {
			promotionPiece = PieceType(tokens[idx].Value[0])
		}
	}

	return &San{
		Piece:          piece,
		FromFile:       fromFile,
		FromRank:       fromRank,
		ToFile:         toFile,
		ToRank:         toRank,
		Capture:        capture,
		EnPassant:      enPassant,
		Check:          check,
		Checkmate:      checkmate,
		PromotionPiece: promotionPiece,
	}, nil
}
