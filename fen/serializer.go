package fen

import (
	"fmt"
	"github.com/jerhon/chess"
	"strings"
)

type FenSerializer struct {
	builder strings.Builder
}

func GetRuneFromChessPiece(piece chess.ChessPiece) rune {

	if piece.Color == chess.WhitePiece {
		switch piece.Piece {
		case chess.Pawn:
			return 'P'
		case chess.Rook:
			return 'R'
		case chess.Knight:
			return 'N'
		case chess.Bishop:
			return 'B'
		case chess.Queen:
			return 'Q'
		case chess.King:
			return 'K'
		}
	} else if piece.Color == chess.BlackPiece {
		switch piece.Piece {
		case chess.Pawn:
			return 'p'
		case chess.Rook:
			return 'r'
		case chess.Knight:
			return 'n'
		case chess.Bishop:
			return 'b'
		case chess.Queen:
			return 'q'
		case chess.King:
			return 'k'
		}
	}
	return ' ' // Return a blank space as a fallback for invalid cases
}

func NewFenSerializer() *FenSerializer {
	return &FenSerializer{
		builder: strings.Builder{},
	}
}

func (this *FenSerializer) String() string {
	return this.builder.String()
}

func (this *FenSerializer) WriteBoard(board *chess.ChessBoard) {
	emptySquares := 0
	for square := range board.IterateSquares() {
		if square.Piece.Piece == chess.NoPiece {
			emptySquares++
		} else {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			this.builder.WriteRune(GetRuneFromChessPiece(square.Piece))
			emptySquares = 0
		}

		if square.Location.File == chess.FileH {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			emptySquares = 0
			if square.Location.Rank != chess.Rank1 {
				this.builder.WriteRune('/')
			}
		}
	}
}

func (this *FenSerializer) WritePlayerToMove(playerColor chess.ColorType) {
	if playerColor == chess.WhitePiece {
		this.builder.WriteString("w")
	} else {
		this.builder.WriteString("b")
	}
}

func (this *FenSerializer) WriteCastlingRights(castlingRights chess.CastlingRights, playerColor chess.ColorType) bool {
	result := false
	if playerColor == chess.WhitePiece {
		if castlingRights.KingSide {
			this.builder.WriteString("K")
			result = true
		}
		if castlingRights.QueenSide {
			this.builder.WriteString("Q")
			result = true
		}
	} else {
		if castlingRights.KingSide {
			this.builder.WriteString("k")
			result = true
		}
		if castlingRights.QueenSide {
			this.builder.WriteString("q")
			result = true
		}
	}

	return result
}

func (this *FenSerializer) WriteEmpty() {
	this.builder.WriteRune('-')
}

func (this *FenSerializer) WriteEnPassantTarget(position chess.ChessLocation) {
	if position.File != chess.NoFile && position.Rank != chess.NoRank {
		this.builder.WriteString(string(position.File))
		this.builder.WriteString(string(position.Rank))
	} else {
		this.builder.WriteRune('-')
	}
}

func (this *FenSerializer) WriteHalfMoveClock(halfMoveClock int) {
	this.builder.WriteString(fmt.Sprintf("%d", halfMoveClock))
}

func (this *FenSerializer) WriteFullMoveNumber(fullMoveNumber int) {
	this.builder.WriteString(fmt.Sprintf("%d", fullMoveNumber))
}

func (this *FenSerializer) WriteSpacer() {
	this.builder.WriteRune(' ')
}

func ToFenString(s *chess.ChessPosition) string {
	fen := NewFenSerializer()

	fen.WriteBoard(s.Board)
	fen.WriteSpacer()
	fen.WritePlayerToMove(s.PlayerToMove)
	fen.WriteSpacer()

	whiteCastlingRightsWritten := fen.WriteCastlingRights(s.WhiteCastlingRights, chess.WhitePiece)
	blackCastlingRightsWritten := fen.WriteCastlingRights(s.BlackCastlingRights, chess.BlackPiece)

	if !blackCastlingRightsWritten && !whiteCastlingRightsWritten {
		fen.WriteEmpty()
	}
	fen.WriteSpacer()

	fen.WriteEnPassantTarget(s.EnPassantSquare)
	fen.WriteSpacer()

	fen.WriteHalfMoveClock(s.HalfmoveClock)
	fen.WriteSpacer()

	fen.WriteFullMoveNumber(s.FullmoveNumber)

	return fen.String()
}
