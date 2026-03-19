package game

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestMove_CastlingRightsPreserved(t *testing.T) {
	tests := []struct {
		name              string
		from              ChessLocation
		to                ChessLocation
		playerToMove      ColorType
		initialRights     map[ColorType]CastlingRights
		expectedRights    map[ColorType]CastlingRights
	}{
		{
			name:         "non-special piece move preserves all castling rights",
			from:         ChessLocation{FileD, Rank2},
			to:           ChessLocation{FileD, Rank4},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "white king move clears white castling rights",
			from:         ChessLocation{FileE, Rank1},
			to:           ChessLocation{FileF, Rank1},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: false, QueenSide: false},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "black king move clears black castling rights",
			from:         ChessLocation{FileE, Rank8},
			to:           ChessLocation{FileF, Rank8},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: false, QueenSide: false},
			},
		},
		{
			name:         "white kingside rook move clears white kingside castling right",
			from:         ChessLocation{FileH, Rank1},
			to:           ChessLocation{FileH, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: false, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "white queenside rook move clears white queenside castling right",
			from:         ChessLocation{FileA, Rank1},
			to:           ChessLocation{FileA, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: false},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "black kingside rook move clears black kingside castling right",
			from:         ChessLocation{FileH, Rank8},
			to:           ChessLocation{FileH, Rank6},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: false, QueenSide: true},
			},
		},
		{
			name:         "black queenside rook move clears black queenside castling right",
			from:         ChessLocation{FileA, Rank8},
			to:           ChessLocation{FileA, Rank6},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: false},
			},
		},
		{
			name:         "rook not on starting rank does not affect castling rights",
			from:         ChessLocation{FileH, Rank5},
			to:           ChessLocation{FileH, Rank3},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "capturing white kingside rook on h1 clears white kingside castling right",
			from:         ChessLocation{FileH, Rank3},
			to:           ChessLocation{FileH, Rank1},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: false, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "capturing white queenside rook on a1 clears white queenside castling right",
			from:         ChessLocation{FileA, Rank3},
			to:           ChessLocation{FileA, Rank1},
			playerToMove: BlackPiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: false},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
		},
		{
			name:         "capturing black kingside rook on h8 clears black kingside castling right",
			from:         ChessLocation{FileH, Rank6},
			to:           ChessLocation{FileH, Rank8},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: false, QueenSide: true},
			},
		},
		{
			name:         "capturing black queenside rook on a8 clears black queenside castling right",
			from:         ChessLocation{FileA, Rank6},
			to:           ChessLocation{FileA, Rank8},
			playerToMove: WhitePiece,
			initialRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: true},
			},
			expectedRights: map[ColorType]CastlingRights{
				WhitePiece: {KingSide: true, QueenSide: true},
				BlackPiece: {KingSide: true, QueenSide: false},
			},
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := NewChessBoard()

			// Place the moving piece on the board
			var piece ChessPiece
			switch {
			case test.from == (ChessLocation{FileE, Rank1}) || test.from == (ChessLocation{FileE, Rank8}):
				piece = ChessPiece{King, test.playerToMove}
			case test.from.File == FileA || test.from.File == FileH:
				piece = ChessPiece{Rook, test.playerToMove}
			default:
				piece = ChessPiece{Pawn, test.playerToMove}
			}
			board.SetSquare(test.from, piece)

			position := &ChessPosition{
				Board:          board,
				PlayerToMove:   test.playerToMove,
				CastlingRights: test.initialRights,
			}

			result := position.Move(test.from, test.to, NoPiece)
			assert.Equal(t, test.expectedRights, result.CastlingRights)
		})
	}
}

// TestMove_BlackKingsideRookFix is a regression test for the bug where
// the black kingside rook check incorrectly used FileE instead of FileH.
func TestMove_BlackKingsideRookFix(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileH, Rank8}, ChessPiece{Rook, BlackPiece})

	position := &ChessPosition{
		Board:        board,
		PlayerToMove: BlackPiece,
		CastlingRights: map[ColorType]CastlingRights{
			WhitePiece: {KingSide: true, QueenSide: true},
			BlackPiece: {KingSide: true, QueenSide: true},
		},
	}

	result := position.Move(ChessLocation{FileH, Rank8}, ChessLocation{FileH, Rank6}, NoPiece)

	// Kingside right must be cleared; queenside must remain
	assert.False(t, result.CastlingRights[BlackPiece].KingSide, "black kingside castling right should be cleared after H8 rook moves")
	assert.True(t, result.CastlingRights[BlackPiece].QueenSide, "black queenside castling right should be preserved")
	// White rights must be untouched
	assert.True(t, result.CastlingRights[WhitePiece].KingSide)
	assert.True(t, result.CastlingRights[WhitePiece].QueenSide)
}

func TestMove_PawnPromotion(t *testing.T) {
	tests := []struct {
		name           string
		color          ColorType
		from           ChessLocation
		to             ChessLocation
		promotionPiece PieceType
	}{
		{
			name:           "white pawn promotes to queen",
			color:          WhitePiece,
			from:           ChessLocation{FileE, Rank7},
			to:             ChessLocation{FileE, Rank8},
			promotionPiece: Queen,
		},
		{
			name:           "white pawn promotes to rook",
			color:          WhitePiece,
			from:           ChessLocation{FileE, Rank7},
			to:             ChessLocation{FileE, Rank8},
			promotionPiece: Rook,
		},
		{
			name:           "white pawn promotes to bishop",
			color:          WhitePiece,
			from:           ChessLocation{FileE, Rank7},
			to:             ChessLocation{FileE, Rank8},
			promotionPiece: Bishop,
		},
		{
			name:           "white pawn promotes to knight",
			color:          WhitePiece,
			from:           ChessLocation{FileE, Rank7},
			to:             ChessLocation{FileE, Rank8},
			promotionPiece: Knight,
		},
		{
			name:           "black pawn promotes to queen",
			color:          BlackPiece,
			from:           ChessLocation{FileD, Rank2},
			to:             ChessLocation{FileD, Rank1},
			promotionPiece: Queen,
		},
		{
			name:           "black pawn promotes to knight",
			color:          BlackPiece,
			from:           ChessLocation{FileD, Rank2},
			to:             ChessLocation{FileD, Rank1},
			promotionPiece: Knight,
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			board := NewChessBoard()
			board.SetSquare(test.from, ChessPiece{Pawn, test.color})

			position := &ChessPosition{
				Board:        board,
				PlayerToMove: test.color,
				CastlingRights: map[ColorType]CastlingRights{
					WhitePiece: {},
					BlackPiece: {},
				},
			}

			result := position.Move(test.from, test.to, test.promotionPiece)

			// Source square must be empty
			fromSquare := result.Board.GetSquare(test.from)
			assert.True(t, fromSquare.IsEmpty(), "source square should be empty after promotion")

			// Destination square must hold the promoted piece with the correct color
			toSquare := result.Board.GetSquare(test.to)
			assert.Equal(t, test.promotionPiece, toSquare.Piece.Piece, "promoted piece type should match")
			assert.Equal(t, test.color, toSquare.Piece.Color, "promoted piece color should match")
		})
	}
}

func TestCalculatePawnMoves_GeneratesPromotionMoves(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank7}, ChessPiece{Pawn, WhitePiece})

	position := &ChessPosition{
		Board:        board,
		PlayerToMove: WhitePiece,
		CastlingRights: map[ColorType]CastlingRights{
			WhitePiece: {},
			BlackPiece: {},
		},
	}

	moves := CalculateMoves(position, ChessLocation{FileE, Rank7})

	// Should generate exactly 4 promotion moves (Q, R, B, N) for forward move
	promotionMoves := []ChessMove{}
	for _, m := range moves {
		if m.To == (ChessLocation{FileE, Rank8}) && m.CanMove {
			promotionMoves = append(promotionMoves, m)
		}
	}
	assert.Len(t, promotionMoves, 4, "should generate 4 promotion moves for forward push")

	pieces := map[PieceType]bool{}
	for _, m := range promotionMoves {
		pieces[m.PromotionPiece] = true
	}
	assert.True(t, pieces[Queen], "should have queen promotion")
	assert.True(t, pieces[Rook], "should have rook promotion")
	assert.True(t, pieces[Bishop], "should have bishop promotion")
	assert.True(t, pieces[Knight], "should have knight promotion")
}

func TestCalculatePawnMoves_GeneratesCapturePromotionMoves(t *testing.T) {
	board := NewChessBoard()
	board.SetSquare(ChessLocation{FileE, Rank7}, ChessPiece{Pawn, WhitePiece})
	// Place a black piece to capture on f8
	board.SetSquare(ChessLocation{FileF, Rank8}, ChessPiece{Rook, BlackPiece})

	position := &ChessPosition{
		Board:        board,
		PlayerToMove: WhitePiece,
		CastlingRights: map[ColorType]CastlingRights{
			WhitePiece: {},
			BlackPiece: {},
		},
	}

	moves := CalculateMoves(position, ChessLocation{FileE, Rank7})

	// Should generate 4 capture-promotion moves toward f8
	capturePromos := []ChessMove{}
	for _, m := range moves {
		if m.To == (ChessLocation{FileF, Rank8}) && m.CanMove {
			capturePromos = append(capturePromos, m)
		}
	}
	assert.Len(t, capturePromos, 4, "should generate 4 capture-promotion moves toward f8")
}
