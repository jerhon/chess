package fen

import (
	"fmt"
	"github.com/jerhon/chess/pkg/chess/game"
	"strings"
)

type FenSerializer struct {
	builder strings.Builder
}

func GetRuneFromChessPiece(piece game.ChessPiece) rune {

	switch piece.Color {
	case game.WhitePiece:
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
	case game.BlackPiece:
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

func (s *FenSerializer) String() string {
	return s.builder.String()
}

func (s *FenSerializer) WriteBoard(chessBoard *game.ChessBoard) {
	emptySquares := 0
	for square := range chessBoard.IterateSquares() {
		if square.Piece.Piece == game.NoPiece {
			emptySquares++
		} else {
			if emptySquares > 0 {
				fmt.Fprintf(&s.builder, "%d", emptySquares)
			}
			s.builder.WriteRune(GetRuneFromChessPiece(square.Piece))
			emptySquares = 0
		}

		if square.Location.File == game.FileH {
			if emptySquares > 0 {
				fmt.Fprintf(&s.builder, "%d", emptySquares)
			}
			emptySquares = 0
			if square.Location.Rank != game.Rank1 {
				s.builder.WriteRune('/')
			}
		}
	}
}

func (s *FenSerializer) WritePlayerToMove(playerColor game.ColorType) {
	if playerColor == game.WhitePiece {
		s.builder.WriteString("w")
	} else {
		s.builder.WriteString("b")
	}
}

func (s *FenSerializer) WriteCastlingRights(castlingRights game.CastlingRights, playerColor game.ColorType) bool {
	result := false
	if playerColor == game.WhitePiece {
		if castlingRights.KingSide {
			s.builder.WriteString("K")
			result = true
		}
		if castlingRights.QueenSide {
			s.builder.WriteString("Q")
			result = true
		}
	} else {
		if castlingRights.KingSide {
			s.builder.WriteString("k")
			result = true
		}
		if castlingRights.QueenSide {
			s.builder.WriteString("q")
			result = true
		}
	}

	return result
}

func (s *FenSerializer) WriteEmpty() {
	s.builder.WriteRune('-')
}

func (s *FenSerializer) WriteEnPassantTarget(position game.ChessLocation) {
	if position.File != game.NoFile && position.Rank != game.NoRank {
		s.builder.WriteString(string(position.File))
		s.builder.WriteString(string(position.Rank))
	} else {
		s.builder.WriteRune('-')
	}
}

func (s *FenSerializer) WriteHalfMoveClock(halfMoveClock int) {
	fmt.Fprintf(&s.builder, "%d", halfMoveClock)
}

func (s *FenSerializer) WriteFullMoveNumber(fullMoveNumber int) {
	fmt.Fprintf(&s.builder, "%d", fullMoveNumber)
}

func (s *FenSerializer) WriteSpacer() {
	s.builder.WriteRune(' ')
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
