package fen

import (
	"fmt"
	game2 "github.com/jerhon/chess/pkg/chess/game"
	"strings"
)

type FenSerializer struct {
	builder strings.Builder
}

func GetRuneFromChessPiece(piece game2.ChessPiece) rune {

	if piece.Color == game2.WhitePiece {
		switch piece.Piece {
		case game2.Pawn:
			return 'P'
		case game2.Rook:
			return 'R'
		case game2.Knight:
			return 'N'
		case game2.Bishop:
			return 'B'
		case game2.Queen:
			return 'Q'
		case game2.King:
			return 'K'
		case game2.NoPiece:
		}
	} else if piece.Color == game2.BlackPiece {
		switch piece.Piece {
		case game2.Pawn:
			return 'p'
		case game2.Rook:
			return 'r'
		case game2.Knight:
			return 'n'
		case game2.Bishop:
			return 'b'
		case game2.Queen:
			return 'q'
		case game2.King:
			return 'k'
		case game2.NoPiece:
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

func (this *FenSerializer) WriteBoard(chessBoard *game2.ChessBoard) {
	emptySquares := 0
	for square := range chessBoard.IterateSquares() {
		if square.Piece.Piece == game2.NoPiece {
			emptySquares++
		} else {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			this.builder.WriteRune(GetRuneFromChessPiece(square.Piece))
			emptySquares = 0
		}

		if square.Location.File == game2.FileH {
			if emptySquares > 0 {
				this.builder.WriteString(fmt.Sprintf("%d", emptySquares))
			}
			emptySquares = 0
			if square.Location.Rank != game2.Rank1 {
				this.builder.WriteRune('/')
			}
		}
	}
}

func (this *FenSerializer) WritePlayerToMove(playerColor game2.ColorType) {
	if playerColor == game2.WhitePiece {
		this.builder.WriteString("w")
	} else {
		this.builder.WriteString("b")
	}
}

func (this *FenSerializer) WriteCastlingRights(castlingRights game2.CastlingRights, playerColor game2.ColorType) bool {
	result := false
	if playerColor == game2.WhitePiece {
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

func (this *FenSerializer) WriteEnPassantTarget(position game2.ChessLocation) {
	if position.File != game2.NoFile && position.Rank != game2.NoRank {
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

func ToFenString(s *game2.ChessPosition) string {
	fen := NewFenSerializer()

	fen.WriteBoard(s.Board)
	fen.WriteSpacer()
	fen.WritePlayerToMove(s.PlayerToMove)
	fen.WriteSpacer()

	whiteCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[game2.WhitePiece], game2.WhitePiece)
	blackCastlingRightsWritten := fen.WriteCastlingRights(s.CastlingRights[game2.BlackPiece], game2.BlackPiece)

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
