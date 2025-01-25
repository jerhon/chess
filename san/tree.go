package san

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
