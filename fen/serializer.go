package fen

import (
	"fmt"
	"github.com/jerhon/chess/board"
	"github.com/jerhon/chess/position"
	"strings"
)

type FenSerializer struct {
	builder strings.Builder
}

func GetRuneFromChessPiece(piece board.ChessPiece) rune {

	if piece.Color == board.WhitePiece {
		switch piece.Piece {
		case board.Pawn:
			return 'P'
		case board.Rook:
			return 'R'
		case board.Knight:
			return 'N'
		case board.Bishop:
			return 'B'
		case board.Queen:
			return 'Q'
		case board.King:
			return 'K'
		case board.NoPiece:
		}
	} else if piece.Color == board.BlackPiece {
		switch piece.Piece {
		case board.Pawn:
			return 'p'
		case board.Rook:
			return 'r'
		case board.Knight:
			return 'n'
		case board.Bishop:
			return 'b'
		case board.Queen:
			return 'q'
		case board.King:
			return 'k'
		case board.NoPiece:
		}
	}
	return '*' // Return a blank space as a fallback for invalid cases
}

func NewFenSerializer() *FenSerializer {
	return &FenSerializer{
		builder: strings.Builder{},
	}
}

func (this *FenSerializer) String() string {
	return this.builder.String()
}

func (this *FenSerializer) WriteBoard(chessBoard *board.ChessBoard) {
	emptySquares := 0
	for square := range chessBoard.IterateSquares() {
		if square.Piece.Piece == board.NoPiece {
			emptySquares++
		} else {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			this.builder.WriteRune(GetRuneFromChessPiece(square.Piece))
			emptySquares = 0
		}

		if square.Location.File == board.FileH {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			emptySquares = 0
			if square.Location.Rank != board.Rank1 {
				this.builder.WriteRune('/')
			}
		}
	}
}

func (this *FenSerializer) WritePlayerToMove(playerColor board.ColorType) {
	if playerColor == board.WhitePiece {
		this.builder.WriteString("w")
	} else {
		this.builder.WriteString("b")
	}
}

func (this *FenSerializer) WriteCastlingRights(castlingRights position.CastlingRights, playerColor board.ColorType) bool {
	result := false
	if playerColor == board.WhitePiece {
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

func (this *FenSerializer) WriteEnPassantTarget(position board.ChessLocation) {
	if position.File != board.NoFile && position.Rank != board.NoRank {
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

func ToFenString(s *position.ChessPosition) string {
	fen := NewFenSerializer()

	fen.WriteBoard(s.Board)
	fen.WriteSpacer()
	fen.WritePlayerToMove(s.PlayerToMove)
	fen.WriteSpacer()

	whiteCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[board.WhitePiece], board.WhitePiece)
	blackCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[board.BlackPiece], board.BlackPiece)

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
