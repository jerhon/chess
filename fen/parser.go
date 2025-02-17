package fen

import (
	"fmt"
	"github.com/jerhon/chess"
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

// ParseBoard parses the first part of a fen string into a chess board with the pieces on the board
func (this *FenParser) ParseBoard() (*chess.ChessBoard, error) {

	board := chess.NewChessBoard()

	rank := chess.Rank8
	file := chess.FileA

	c, _, err := this.reader.ReadRune()
	for err == nil && c != ' ' {
		if c == '/' {
			rank--
			file = chess.FileA
		} else if unicode.IsDigit(c) {
			file += chess.FileType(c - '0')
		} else if unicode.IsLetter(c) {
			piece, err := GetChessPieceFromFenRune(c)
			if err != nil {
				return nil, err
			}
			board.SetSquare(chess.FileType(file), chess.RankType(rank), piece)
			file++
		}

		c, _, err = this.reader.ReadRune()
	}

	return board, nil
}

// GetChessPieceFromFenRune returns a chess piece based on a character from a fen string
func GetChessPieceFromFenRune(r rune) (chess.ChessPiece, error) {
	switch r {
	case 'P':
		return chess.ChessPiece{Piece: chess.Pawn, Color: chess.WhitePiece}, nil
	case 'R':
		return chess.ChessPiece{Piece: chess.Rook, Color: chess.WhitePiece}, nil
	case 'N':
		return chess.ChessPiece{Piece: chess.Knight, Color: chess.WhitePiece}, nil
	case 'B':
		return chess.ChessPiece{Piece: chess.Bishop, Color: chess.WhitePiece}, nil
	case 'Q':
		return chess.ChessPiece{Piece: chess.Queen, Color: chess.WhitePiece}, nil
	case 'K':
		return chess.ChessPiece{Piece: chess.King, Color: chess.WhitePiece}, nil
	case 'p':
		return chess.ChessPiece{Piece: chess.Pawn, Color: chess.BlackPiece}, nil
	case 'r':
		return chess.ChessPiece{Piece: chess.Rook, Color: chess.BlackPiece}, nil
	case 'n':
		return chess.ChessPiece{Piece: chess.Knight, Color: chess.BlackPiece}, nil
	case 'b':
		return chess.ChessPiece{Piece: chess.Bishop, Color: chess.BlackPiece}, nil
	case 'q':
		return chess.ChessPiece{Piece: chess.Queen, Color: chess.BlackPiece}, nil
	case 'k':
		return chess.ChessPiece{Piece: chess.King, Color: chess.BlackPiece}, nil
	default:
		return chess.ChessPiece{}, fmt.Errorf("invalid Fen piece character: %c", r)
	}
}

func (this *FenParser) ParsePlayerToMove() (chess.ColorType, error) {
	fenPlayerToMove, _, err := this.reader.ReadRune()
	if err != nil {
		return chess.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', but reached end of string")
	}

	piece := chess.NoColor
	switch fenPlayerToMove {
	case 'w':
		piece = chess.WhitePiece
	case 'b':

		piece = chess.BlackPiece
	default:
		return chess.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', saw: %c", fenPlayerToMove)
	}

	// Advance past whitespace if it exists
	fenPlayerToMove, _, err = this.reader.ReadRune()
	if err == nil && fenPlayerToMove != ' ' {
		return chess.NoColor, fmt.Errorf("invalid FEN player to move, expected ' ' after player to move, saw: %c", fenPlayerToMove)
	}

	return piece, nil
}

func (this *FenParser) ParseCastlingRights() (whiteCastlingRights chess.CastlingRights, blackCastlingRights chess.CastlingRights, err error) {
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
				return chess.CastlingRights{}, chess.CastlingRights{}, fmt.Errorf("invalid FEN castling rights, expected 'K', 'Q', 'k', or 'q', saw: %c", fenCastlingRights)
			}

			fenCastlingRights, _, err = this.reader.ReadRune()
		}
	} else {

		space, _, err := this.reader.ReadRune()
		if err == nil && space != ' ' {
			return chess.CastlingRights{}, chess.CastlingRights{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
		}
	}

	return whiteCastlingRights, blackCastlingRights, nil
}

func ParseFile(r rune) (chess.FileType, error) {
	if r >= 'a' && r <= 'h' {
		return chess.FileType(r), nil
	} else if r >= 'A' && r <= 'H' {
		return chess.FileType(r - 'A' + 'a'), nil
	} else {
		return chess.NoFile, fmt.Errorf("invalid FEN file character: %c", r)
	}
}

func ParseRank(r rune) (chess.RankType, error) {
	if r >= '1' && r <= '8' {
		return chess.RankType(r), nil
	} else {
		return chess.NoRank, fmt.Errorf("invalid FEN rank character: %c", r)
	}
}

func (this *FenParser) ParseEnPassantTarget() (chess.ChessLocation, error) {
	enPassantTarget, _, err := this.reader.ReadRune()

	chessLocation := chess.ChessLocation{}
	if err != nil {
		return chessLocation, nil
	}

	if enPassantTarget != '-' {
		file, err := ParseFile(enPassantTarget)
		if err != nil {
			return chess.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected file: %c", enPassantTarget)
		}

		enPassantTarget, _, err = this.reader.ReadRune()
		if err != nil {
			return chess.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		rank, err := ParseRank(enPassantTarget)
		if err != nil {
			return chess.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		chessLocation.File = file
		chessLocation.Rank = rank
	}

	space, _, err := this.reader.ReadRune()
	if err == nil && space != ' ' {
		return chess.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
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

func ParseFen(fenString string) (chess.ChessPosition, error) {
	parser := NewFenParser(fenString)
	board, err := parser.ParseBoard()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	playerToMove, err := parser.ParsePlayerToMove()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	whiteCastlingRights, blackCastlingRights, err := parser.ParseCastlingRights()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	enPassantTarget, err := parser.ParseEnPassantTarget()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	halfMoveClock, err := parser.ParseHalfMoveClock()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	fullMoveNumber, err := parser.ParseFullMoveNumber()
	if err != nil {
		return chess.ChessPosition{}, err
	}

	return chess.ChessPosition{
		Board:               board,
		PlayerToMove:        playerToMove,
		BlackCastlingRights: blackCastlingRights,
		WhiteCastlingRights: whiteCastlingRights,
		EnPassantSquare:     enPassantTarget,
		HalfmoveClock:       halfMoveClock,
		FullmoveNumber:      fullMoveNumber,
	}, nil
}
