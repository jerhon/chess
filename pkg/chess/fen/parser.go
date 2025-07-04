package fen

import (
	"fmt"
	game2 "github.com/jerhon/chess/pkg/chess/game"
	"strings"
	"unicode"
)

// FenParser parses a fen string into various chess pieces preserving the location in the string
type FenParser struct {
	reader *strings.Reader
}

func NewFenParser(fen string) *FenParser {
	return &FenParser{
		reader: strings.NewReader(fen),
	}
}

// ParseBoard parses the first part of a fen string into a chess game with the pieces on the game
func (this *FenParser) ParseBoard() (*game2.ChessBoard, error) {

	chessBoard := game2.NewChessBoard()

	rank := game2.Rank8
	file := game2.FileA

	c, _, err := this.reader.ReadRune()
	for err == nil && c != ' ' {
		if c == '/' {
			rank--
			file = game2.FileA
		} else if unicode.IsDigit(c) {
			file += game2.FileType(c - '0')
		} else if unicode.IsLetter(c) {
			piece, err := GetChessPieceFromFenRune(c)
			if err != nil {
				return nil, err
			}
			chessBoard.SetSquare(game2.ChessLocation{File: game2.FileType(file), Rank: game2.RankType(rank)}, piece)
			file++
		}

		c, _, err = this.reader.ReadRune()
	}

	return chessBoard, nil
}

// GetChessPieceFromFenRune returns a chess piece based on a character from a fen string
func GetChessPieceFromFenRune(r rune) (game2.ChessPiece, error) {
	switch r {
	case 'P':
		return game2.ChessPiece{Piece: game2.Pawn, Color: game2.WhitePiece}, nil
	case 'R':
		return game2.ChessPiece{Piece: game2.Rook, Color: game2.WhitePiece}, nil
	case 'N':
		return game2.ChessPiece{Piece: game2.Knight, Color: game2.WhitePiece}, nil
	case 'B':
		return game2.ChessPiece{Piece: game2.Bishop, Color: game2.WhitePiece}, nil
	case 'Q':
		return game2.ChessPiece{Piece: game2.Queen, Color: game2.WhitePiece}, nil
	case 'K':
		return game2.ChessPiece{Piece: game2.King, Color: game2.WhitePiece}, nil
	case 'p':
		return game2.ChessPiece{Piece: game2.Pawn, Color: game2.BlackPiece}, nil
	case 'r':
		return game2.ChessPiece{Piece: game2.Rook, Color: game2.BlackPiece}, nil
	case 'n':
		return game2.ChessPiece{Piece: game2.Knight, Color: game2.BlackPiece}, nil
	case 'b':
		return game2.ChessPiece{Piece: game2.Bishop, Color: game2.BlackPiece}, nil
	case 'q':
		return game2.ChessPiece{Piece: game2.Queen, Color: game2.BlackPiece}, nil
	case 'k':
		return game2.ChessPiece{Piece: game2.King, Color: game2.BlackPiece}, nil
	default:
		return game2.ChessPiece{}, fmt.Errorf("invalid Fen piece character: %c", r)
	}
}

func (this *FenParser) ParsePlayerToMove() (game2.ColorType, error) {
	fenPlayerToMove, _, err := this.reader.ReadRune()
	if err != nil {
		return game2.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', but reached end of string")
	}

	piece := game2.NoColor
	switch fenPlayerToMove {
	case 'w':
		piece = game2.WhitePiece
	case 'b':

		piece = game2.BlackPiece
	default:
		return game2.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', saw: %c", fenPlayerToMove)
	}

	// Advance past whitespace if it exists
	fenPlayerToMove, _, err = this.reader.ReadRune()
	if err == nil && fenPlayerToMove != ' ' {
		return game2.NoColor, fmt.Errorf("invalid FEN player to move, expected ' ' after player to move, saw: %c", fenPlayerToMove)
	}

	return piece, nil
}

func (this *FenParser) ParseCastlingRights() (whiteCastlingRights game2.CastlingRights, blackCastlingRights game2.CastlingRights, err error) {
	fenCastlingRights, _, err := this.reader.ReadRune()
	if fenCastlingRights != '-' {
		for fenCastlingRights != ' ' && err == nil {
			switch fenCastlingRights {
			case 'K':
				whiteCastlingRights.KingSide = true
			case 'Q':
				whiteCastlingRights.QueenSide = true
			case 'k':
				blackCastlingRights.KingSide = true
			case 'q':
				blackCastlingRights.QueenSide = true
			default:
				return game2.CastlingRights{}, game2.CastlingRights{}, fmt.Errorf("invalid FEN castling rights, expected 'K', 'Q', 'k', or 'q', saw: %c", fenCastlingRights)
			}

			fenCastlingRights, _, err = this.reader.ReadRune()
		}
	} else {

		space, _, err := this.reader.ReadRune()
		if err == nil && space != ' ' {
			return game2.CastlingRights{}, game2.CastlingRights{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
		}
	}

	return whiteCastlingRights, blackCastlingRights, nil
}

func ParseFile(r rune) (game2.FileType, error) {
	if r >= 'a' && r <= 'h' {
		return game2.FileType(r), nil
	} else if r >= 'A' && r <= 'H' {
		return game2.FileType(r - 'A' + 'a'), nil
	} else {
		return game2.NoFile, fmt.Errorf("invalid FEN file character: %c", r)
	}
}

func ParseRank(r rune) (game2.RankType, error) {
	if r >= '1' && r <= '8' {
		return game2.RankType(r), nil
	} else {
		return game2.NoRank, fmt.Errorf("invalid FEN rank character: %c", r)
	}
}

func (this *FenParser) ParseEnPassantTarget() (game2.ChessLocation, error) {
	enPassantTarget, _, err := this.reader.ReadRune()

	chessLocation := game2.ChessLocation{}
	if err != nil {
		return chessLocation, nil
	}

	if enPassantTarget != '-' {
		file, err := ParseFile(enPassantTarget)
		if err != nil {
			return game2.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected file: %c", enPassantTarget)
		}

		enPassantTarget, _, err = this.reader.ReadRune()
		if err != nil {
			return game2.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		rank, err := ParseRank(enPassantTarget)
		if err != nil {
			return game2.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		chessLocation.File = file
		chessLocation.Rank = rank
	}

	space, _, err := this.reader.ReadRune()
	if err == nil && space != ' ' {
		return game2.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
	}

	return chessLocation, nil
}

func (this *FenParser) ParseHalfMoveClock() (int, error) {
	fenHalfMoveClock, _, err := this.reader.ReadRune()
	if err != nil {
		return 0, nil
	}

	if fenHalfMoveClock == '-' {
		return 0, nil
	}

	halfClock := 0
	for unicode.IsDigit(fenHalfMoveClock) && err == nil {
		halfClock = halfClock*10 + int(fenHalfMoveClock-'0')
		fenHalfMoveClock, _, err = this.reader.ReadRune()
	}

	if err == nil && fenHalfMoveClock != ' ' {
		return 0, fmt.Errorf("invalid FEN half move clock, expected space after number, saw: %c", fenHalfMoveClock)
	}

	return halfClock, nil
}

func (this *FenParser) ParseFullMoveNumber() (int, error) {
	fenFullMoveNumber, _, err := this.reader.ReadRune()
	if err != nil {
		return 0, nil
	}

	if fenFullMoveNumber == '-' {
		return 0, nil
	}

	fullMoveNumber := 0
	for unicode.IsDigit(fenFullMoveNumber) && err == nil {
		fullMoveNumber = fullMoveNumber*10 + int(fenFullMoveNumber-'0')
		fenFullMoveNumber, _, err = this.reader.ReadRune()
	}

	if err == nil && fenFullMoveNumber != ' ' {
		return 0, fmt.Errorf("invalid FEN full move number, expected space after number, saw: %c", fenFullMoveNumber)
	}

	return fullMoveNumber, nil
}

func ParseFen(fenString string) (game2.ChessPosition, error) {
	parser := NewFenParser(fenString)
	chessBoard, err := parser.ParseBoard()

	if err != nil {
		return game2.ChessPosition{}, err
	}

	playerToMove, err := parser.ParsePlayerToMove()
	if err != nil {
		return game2.ChessPosition{}, err
	}

	whiteCastlingRights, blackCastlingRights, err := parser.ParseCastlingRights()
	if err != nil {
		return game2.ChessPosition{}, err
	}

	enPassantTarget, err := parser.ParseEnPassantTarget()
	if err != nil {
		return game2.ChessPosition{}, err
	}

	halfMoveClock, err := parser.ParseHalfMoveClock()
	if err != nil {
		return game2.ChessPosition{}, err
	}

	fullMoveNumber, err := parser.ParseFullMoveNumber()
	if err != nil {
		return game2.ChessPosition{}, err
	}

	return game2.ChessPosition{
		Board:        chessBoard,
		PlayerToMove: playerToMove,
		CastlingRights: map[game2.ColorType]game2.CastlingRights{
			game2.WhitePiece: whiteCastlingRights,
			game2.BlackPiece: blackCastlingRights,
		},
		EnPassantSquare: enPassantTarget,
		HalfmoveClock:   halfMoveClock,
		FullmoveNumber:  fullMoveNumber,
	}, nil
}
