package fen

import (
	"fmt"
	"github.com/jerhon/chess"
	"strings"
	"unicode"
)

// This file parses Fen strings into a Chess Game representation

func Parse(fen string) {

}

// ParsePositions parses the first part of a fen string into a chess board with the pieces on the board
func ParsePositions(fenPositions string) (*chess.ChessBoard, error) {

	board := &chess.ChessBoard{}

	reader := strings.NewReader(fenPositions)

	for i := 0; i < len(fenPositions); i++ {
		rank := '8'
		file := 'a'

		r, _, err := reader.ReadRune()
		if err != nil {
			return board, nil
		}

		if r == '/' {
			rank--
			file = 'a'
		} else if unicode.IsDigit(r) {
			file += (r - '0')
		} else if unicode.IsLetter(r) {
			piece, err := GetChessPiece(r)
			if err != nil {
				return nil, err
			}
			board.SetSquare(chess.FileType(file), chess.RankType(rank), piece)
		}
	}

	return board, nil
}

// GetChessPiece returns a chess piece based on a character from a fen string
func GetChessPiece(r rune) (chess.ChessPiece, error) {
	switch r {
	case 'P':
		return chess.ChessPiece{Piece: chess.Pawn, Color: chess.BlackPiece}, nil
	case 'R':
		return chess.ChessPiece{Piece: chess.Rook, Color: chess.BlackPiece}, nil
	case 'N':
		return chess.ChessPiece{Piece: chess.Knight, Color: chess.BlackPiece}, nil
	case 'B':
		return chess.ChessPiece{Piece: chess.Bishop, Color: chess.BlackPiece}, nil
	case 'Q':
		return chess.ChessPiece{Piece: chess.Queen, Color: chess.BlackPiece}, nil
	case 'K':
		return chess.ChessPiece{Piece: chess.King, Color: chess.BlackPiece}, nil
	case 'p':
		return chess.ChessPiece{Piece: chess.Pawn, Color: chess.WhitePiece}, nil
	case 'r':
		return chess.ChessPiece{Piece: chess.Rook, Color: chess.WhitePiece}, nil
	case 'n':
		return chess.ChessPiece{Piece: chess.Knight, Color: chess.WhitePiece}, nil
	case 'b':
		return chess.ChessPiece{Piece: chess.Bishop, Color: chess.WhitePiece}, nil
	case 'q':
		return chess.ChessPiece{Piece: chess.Queen, Color: chess.WhitePiece}, nil
	case 'k':
		return chess.ChessPiece{Piece: chess.King, Color: chess.WhitePiece}, nil
	default:
		return chess.ChessPiece{}, fmt.Errorf("invalid Fen piece character: %c", r)
	}
}
