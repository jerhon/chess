package fen

import (
	"fmt"
	"github.com/jerhon/chess/board"
	"github.com/jerhon/chess/position"
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
func (this *FenParser) ParseBoard() (*board.ChessBoard, error) {

	chessBoard := board.NewChessBoard()

	rank := board.Rank8
	file := board.FileA

	c, _, err := this.reader.ReadRune()
	for err == nil && c != ' ' {
		if c == '/' {
			rank--
			file = board.FileA
		} else if unicode.IsDigit(c) {
			file += board.FileType(c - '0')
		} else if unicode.IsLetter(c) {
			piece, err := GetChessPieceFromFenRune(c)
			if err != nil {
				return nil, err
			}
			chessBoard.SetSquare(board.ChessLocation{File: board.FileType(file), Rank: board.RankType(rank)}, piece)
			file++
		}

		c, _, err = this.reader.ReadRune()
	}

	return chessBoard, nil
}

// GetChessPieceFromFenRune returns a chess piece based on a character from a fen string
func GetChessPieceFromFenRune(r rune) (board.ChessPiece, error) {
	switch r {
	case 'P':
		return board.ChessPiece{Piece: board.Pawn, Color: board.WhitePiece}, nil
	case 'R':
		return board.ChessPiece{Piece: board.Rook, Color: board.WhitePiece}, nil
	case 'N':
		return board.ChessPiece{Piece: board.Knight, Color: board.WhitePiece}, nil
	case 'B':
		return board.ChessPiece{Piece: board.Bishop, Color: board.WhitePiece}, nil
	case 'Q':
		return board.ChessPiece{Piece: board.Queen, Color: board.WhitePiece}, nil
	case 'K':
		return board.ChessPiece{Piece: board.King, Color: board.WhitePiece}, nil
	case 'p':
		return board.ChessPiece{Piece: board.Pawn, Color: board.BlackPiece}, nil
	case 'r':
		return board.ChessPiece{Piece: board.Rook, Color: board.BlackPiece}, nil
	case 'n':
		return board.ChessPiece{Piece: board.Knight, Color: board.BlackPiece}, nil
	case 'b':
		return board.ChessPiece{Piece: board.Bishop, Color: board.BlackPiece}, nil
	case 'q':
		return board.ChessPiece{Piece: board.Queen, Color: board.BlackPiece}, nil
	case 'k':
		return board.ChessPiece{Piece: board.King, Color: board.BlackPiece}, nil
	default:
		return board.ChessPiece{}, fmt.Errorf("invalid Fen piece character: %c", r)
	}
}

func (this *FenParser) ParsePlayerToMove() (board.ColorType, error) {
	fenPlayerToMove, _, err := this.reader.ReadRune()
	if err != nil {
		return board.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', but reached end of string")
	}

	piece := board.NoColor
	switch fenPlayerToMove {
	case 'w':
		piece = board.WhitePiece
	case 'b':

		piece = board.BlackPiece
	default:
		return board.NoColor, fmt.Errorf("invalid FEN player to move, expected 'w' or 'b', saw: %c", fenPlayerToMove)
	}

	// Advance past whitespace if it exists
	fenPlayerToMove, _, err = this.reader.ReadRune()
	if err == nil && fenPlayerToMove != ' ' {
		return board.NoColor, fmt.Errorf("invalid FEN player to move, expected ' ' after player to move, saw: %c", fenPlayerToMove)
	}

	return piece, nil
}

func (this *FenParser) ParseCastlingRights() (whiteCastlingRights position.CastlingRights, blackCastlingRights position.CastlingRights, err error) {
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
				return position.CastlingRights{}, position.CastlingRights{}, fmt.Errorf("invalid FEN castling rights, expected 'K', 'Q', 'k', or 'q', saw: %c", fenCastlingRights)
			}

			fenCastlingRights, _, err = this.reader.ReadRune()
		}
	} else {

		space, _, err := this.reader.ReadRune()
		if err == nil && space != ' ' {
			return position.CastlingRights{}, position.CastlingRights{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
		}
	}

	return whiteCastlingRights, blackCastlingRights, nil
}

func ParseFile(r rune) (board.FileType, error) {
	if r >= 'a' && r <= 'h' {
		return board.FileType(r), nil
	} else if r >= 'A' && r <= 'H' {
		return board.FileType(r - 'A' + 'a'), nil
	} else {
		return board.NoFile, fmt.Errorf("invalid FEN file character: %c", r)
	}
}

func ParseRank(r rune) (board.RankType, error) {
	if r >= '1' && r <= '8' {
		return board.RankType(r), nil
	} else {
		return board.NoRank, fmt.Errorf("invalid FEN rank character: %c", r)
	}
}

func (this *FenParser) ParseEnPassantTarget() (board.ChessLocation, error) {
	enPassantTarget, _, err := this.reader.ReadRune()

	chessLocation := board.ChessLocation{}
	if err != nil {
		return chessLocation, nil
	}

	if enPassantTarget != '-' {
		file, err := ParseFile(enPassantTarget)
		if err != nil {
			return board.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected file: %c", enPassantTarget)
		}

		enPassantTarget, _, err = this.reader.ReadRune()
		if err != nil {
			return board.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		rank, err := ParseRank(enPassantTarget)
		if err != nil {
			return board.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected rank after file, saw: %c", enPassantTarget)
		}

		chessLocation.File = file
		chessLocation.Rank = rank
	}

	space, _, err := this.reader.ReadRune()
	if err == nil && space != ' ' {
		return board.ChessLocation{}, fmt.Errorf("invalid FEN en passant target, expected space after rank, saw: %c", space)
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

func ParseFen(fenString string) (position.ChessPosition, error) {
	parser := NewFenParser(fenString)
	chessBoard, err := parser.ParseBoard()

	if err != nil {
		return position.ChessPosition{}, err
	}

	playerToMove, err := parser.ParsePlayerToMove()
	if err != nil {
		return position.ChessPosition{}, err
	}

	whiteCastlingRights, blackCastlingRights, err := parser.ParseCastlingRights()
	if err != nil {
		return position.ChessPosition{}, err
	}

	enPassantTarget, err := parser.ParseEnPassantTarget()
	if err != nil {
		return position.ChessPosition{}, err
	}

	halfMoveClock, err := parser.ParseHalfMoveClock()
	if err != nil {
		return position.ChessPosition{}, err
	}

	fullMoveNumber, err := parser.ParseFullMoveNumber()
	if err != nil {
		return position.ChessPosition{}, err
	}

	return position.ChessPosition{
		Board:        chessBoard,
		PlayerToMove: playerToMove,
		CastlingRights: map[board.ColorType]position.CastlingRights{
			board.WhitePiece: whiteCastlingRights,
			board.BlackPiece: blackCastlingRights,
		},
		EnPassantSquare: enPassantTarget,
		HalfmoveClock:   halfMoveClock,
		FullmoveNumber:  fullMoveNumber,
	}, nil
}
