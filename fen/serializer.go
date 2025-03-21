package fen

import (
	"fmt"
	"github.com/jerhon/chess/game"
	"strings"
)

type FenSerializer struct {
	builder strings.Builder
}

func GetRuneFromChessPiece(piece game.ChessPiece) rune {

	if piece.Color == game.WhitePiece {
		switch piece.Piece {
		case game.Pawn:
			return 'P'
		case game.Rook:
			return 'R'
		case game.Knight:
			return 'N'
		case game.Bishop:
			return 'B'
		case game.Queen:
			return 'Q'
		case game.King:
			return 'K'
		case game.NoPiece:
		}
	} else if piece.Color == game.BlackPiece {
		switch piece.Piece {
		case game.Pawn:
			return 'p'
		case game.Rook:
			return 'r'
		case game.Knight:
			return 'n'
		case game.Bishop:
			return 'b'
		case game.Queen:
			return 'q'
		case game.King:
			return 'k'
		case game.NoPiece:
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

func (this *FenSerializer) WriteBoard(chessBoard *game.ChessBoard) {
	emptySquares := 0
	for square := range chessBoard.IterateSquares() {
		if square.Piece.Piece == game.NoPiece {
			emptySquares++
		} else {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			this.builder.WriteRune(GetRuneFromChessPiece(square.Piece))
			emptySquares = 0
		}

		if square.Location.File == game.FileH {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			emptySquares = 0
			if square.Location.Rank != game.Rank1 {
				this.builder.WriteRune('/')
			}
		}
	}
}

func (this *FenSerializer) WritePlayerToMove(playerColor game.ColorType) {
	if playerColor == game.WhitePiece {
		this.builder.WriteString("w")
	} else {
		this.builder.WriteString("b")
	}
}

func (this *FenSerializer) WriteCastlingRights(castlingRights game.CastlingRights, playerColor game.ColorType) bool {
	result := false
	if playerColor == game.WhitePiece {
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

func (this *FenSerializer) WriteEnPassantTarget(position game.ChessLocation) {
	if position.File != game.NoFile && position.Rank != game.NoRank {
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

func ToFenString(s *game.ChessPosition) string {
	fen := NewFenSerializer()

	fen.WriteBoard(s.Board)
	fen.WriteSpacer()
	fen.WritePlayerToMove(s.PlayerToMove)
	fen.WriteSpacer()

	whiteCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[game.WhitePiece], game.WhitePiece)
	blackCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[game.BlackPiece], game.BlackPiece)

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
