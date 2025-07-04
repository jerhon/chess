package san

import (
	"github.com/jerhon/chess/pkg/chess/game"
)

// ParseSan parses a san string returning either a san move or a castling move
func ParseSan(san string) (*SanMove, *SanCastle, error) {
	tokens, err := ParseSanTokens(san)
	if err != nil {
		return nil, nil, err
	}

	piece := game.Pawn
	fromFile := game.NoFile
	fromRank := game.NoRank
	toFile := game.NoFile
	toRank := game.NoRank
	capture := false
	enPassant := false
	check := false
	checkmate := false
	promotionPiece := game.NoPiece

	// Special case for castling
	if san == "O-O" || san == "0-0" {
		return nil, &SanCastle{
			CastleKingSide: true,
		}, nil
	} else if san == "O-O-O" || san == "0-0-0" {
		return nil, &SanCastle{
			CastleQueenSide: true,
		}, nil
	}

	idx := 0
	if idx < len(tokens) && tokens[idx].Type == PieceToken {
		piece = game.PieceType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		toFile = game.FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		toRank = game.RankType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == CaptureToken {
		capture = true
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == FileToken {
		fromFile = toFile
		toFile = game.FileType(tokens[idx].Value[0])
		idx++
	}

	if idx < len(tokens) && tokens[idx].Type == RankToken {
		fromRank = toRank
		toRank = game.RankType(tokens[idx].Value[0])
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
			promotionPiece = game.PieceType(tokens[idx].Value[0])
		}
	}

	return &SanMove{
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
	}, nil, nil
}
