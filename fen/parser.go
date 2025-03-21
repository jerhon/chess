package fen

import (
	"fmt"
	"github.com/jerhon/chess/game"
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
func (this *FenParser) ParseBoard() (*game.ChessBoard, error) {

	chessBoard := game.NewChessBoard()

	rank := game.Rank8
	file := game.FileA

	c, _, err := this.reader.ReadRune()
	for err == nil && c != ' ' {
		if c == '/' {
			rank--
			file = game.FileA
		} else if unicode.IsDigit(c) {
			file += game.FileType(c - '0')
		} else if unicode.IsLetter(c) {
			piece, err := GetChessPieceFromFenRune(c)
			if err != nil {
				return nil, err
			}
			chessBoard.SetSquare(game.ChessLocation{File: game.FileType(file), Rank: game.RankType(rank)}, piece)
			file++
		}

		c, _, err = this.reader.ReadRune()
	}

	return chessBoard, nil
}

// GetChessPieceFromFenRune returns a chess piece based on a character from a fen string
func GetChessPieceFromFenRune(r rune) (game.ChessPiece, error) {
	switch r {
	case 'P':
		return game.ChessPiece{Piece: game.Pawn, Color: game.WhitePiece}, nil
	case 'R':
		return game.ChessPiece{Piece: game.Rook, Color: game.WhitePiece}, nil
	case 'N':
		return game.ChessPiece{Piece: game.Knight, Color: game.WhitePiece}, nil
	case 'B':
		return game.ChessPiece{Piece: game.Bishop, Color: game.WhitePiece}, nil
	case 'Q':
		return game.ChessPiece{Piece: game.Queen, Color: game.WhitePiece}, nil
	case 'K':
		return game.ChessPiece{Piece: game.King, Color: game.WhitePiece}, nil
	case 'p':
		return game.ChessPiece{Piece: game.Pawn, Color: game.BlackPiece}, nil
	case 'r':
		return game.ChessPiece{Piece: game.Rook, Color: game.BlackPiece}, nil
	case 'n':
		return game.ChessPiece{Piece: game.Knight, Color: game.BlackPiece}, nil
	case 'b':
		return game.ChessPiece{Piece: game.Bishop, Color: game.BlackPiece}, nil
	case 'q':
		return game.ChessPiece{Piece: game.Queen, Color: game.BlackPiece}, nil
	case 'k':
		return game.ChessPiece{Piece: game.King, Color: game.BlackPiece}, nil
	default:
		return game.ChessPiece{}, fmt.Errorf("invalid Fen piece character: %c", r)
	}
}

func (this *FenParser) ParsePlayerToMove() (game.ColorType, error) {
	fenPlayerToMove, _, err := this.reader.ReadRune()
	if err != nil {
		return game.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', but reached end of string")
	}

	piece := game.NoColor
	switch fenPlayerToMove {
	case 'w':
		piece = game.WhitePiece
	case 'b':

		piece = game.BlackPiece
	default:
		return game.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', saw: %c", fenPlayerToMove)
	}

	// Advance past whitespace if it exists
	fenPlayerToMove, _, err = this.reader.ReadRune()
	if err == nil && fenPlayerToMove != ' ' {
		return game.NoColor, fmt.Errorf("invalid FEN player to move, expected ' ' after player to move, saw: %c", fenPlayerToMove)
	}

	return piece, nil
}

func (this *FenParser) ParseCastlingRights() (whiteCastlingRights game.CastlingRights, blackCastlingRights game.CastlingRights, err error) {
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
				return game.CastlingRights{}, game.CastlingRights{}, fmt.Errorf("invalid FEN castling rights, expected 'K', 'Q', 'k', or 'q', saw: %c", fenCastlingRights)
			}

			fenCastlingRights, _, err = this.reader.ReadRune()
		}
	} else {

		space, _, err := this.reader.ReadRune()
		if err == nil && space != ' ' {
			return game.CastlingRights{}, game.CastlingRights{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
		}
	}

	return whiteCastlingRights, blackCastlingRights, nil
}

func ParseFile(r rune) (game.FileType, error) {
	if r >= 'a' && r <= 'h' {
		return game.FileType(r), nil
	} else if r >= 'A' && r <= 'H' {
		return game.FileType(r - 'A' + 'a'), nil
	} else {
		return game.NoFile, fmt.Errorf("invalid FEN file character: %c", r)
	}
}

func ParseRank(r rune) (game.RankType, error) {
	if r >= '1' && r <= '8' {
		return game.RankType(r), nil
	} else {
		return game.NoRank, fmt.Errorf("invalid FEN rank character: %c", r)
	}
}

func (this *FenParser) ParseEnPassantTarget() (game.ChessLocation, error) {
	enPassantTarget, _, err := this.reader.ReadRune()

	chessLocation := game.ChessLocation{}
	if err != nil {
		return chessLocation, nil
	}

	if enPassantTarget != '-' {
		file, err := ParseFile(enPassantTarget)
		if err != nil {
			return game.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected file: %c", enPassantTarget)
		}

		enPassantTarget, _, err = this.reader.ReadRune()
		if err != nil {
			return game.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		rank, err := ParseRank(enPassantTarget)
		if err != nil {
			return game.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		chessLocation.File = file
		chessLocation.Rank = rank
	}

	space, _, err := this.reader.ReadRune()
	if err == nil && space != ' ' {
		return game.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
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

func ParseFen(fenString string) (game.ChessPosition, error) {
	parser := NewFenParser(fenString)
	chessBoard, err := parser.ParseBoard()

	if err != nil {
		return game.ChessPosition{}, err
	}

	playerToMove, err := parser.ParsePlayerToMove()
	if err != nil {
		return game.ChessPosition{}, err
	}

	whiteCastlingRights, blackCastlingRights, err := parser.ParseCastlingRights()
	if err != nil {
		return game.ChessPosition{}, err
	}

	enPassantTarget, err := parser.ParseEnPassantTarget()
	if err != nil {
		return game.ChessPosition{}, err
	}

	halfMoveClock, err := parser.ParseHalfMoveClock()
	if err != nil {
		return game.ChessPosition{}, err
	}

	fullMoveNumber, err := parser.ParseFullMoveNumber()
	if err != nil {
		return game.ChessPosition{}, err
	}

	return game.ChessPosition{
		Board:        chessBoard,
		PlayerToMove: playerToMove,
		CastlingRights: map[game.ColorType]game.CastlingRights{
			game.WhitePiece: whiteCastlingRights,
			game.BlackPiece: blackCastlingRights,
		},
		EnPassantSquare: enPassantTarget,
		HalfmoveClock:   halfMoveClock,
		FullmoveNumber:  fullMoveNumber,
	}, nil
}
