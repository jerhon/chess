package san

import (
	"github.com/jerhon/chess/board"
)

func ParseSan(san string) (*San, error) {
	tokens, err := ParseSanTokens(san)
	if err != nil {
		return nil, err
	}

	piece := board.Pawn
	fromFile := board.NoFile
	fromRank := board.NoRank
	toFile := board.NoFile
	toRank := board.NoRank
	capture := false
	enPassant := false
	check := false
	checkmate := false
	promotionPiece := board.NoPiece

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
		piece = board.PieceType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		toFile = board.FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		toRank = board.RankType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == CaptureToken {
		capture = true
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		fromFile = toFile
		toFile = board.FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		fromRank = toRank
		toRank = board.RankType(tokens[idx].Value[0])
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
			promotionPiece = board.PieceType(tokens[idx].Value[0])
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
