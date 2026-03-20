package chess

import (
	"testing"

	"github.com/jerhon/chess/pkg/chess/fen"
	"github.com/jerhon/chess/pkg/chess/game"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestGetLegalMoves_NotNilOnNewGame(t *testing.T) {
	g := NewGame()
	moves := g.GetLegalMoves()
	assert.NotNil(t, moves)
	assert.NotEmpty(t, moves)
}

func TestGetLegalMoves_AllMovesCanBeExecuted(t *testing.T) {
	g := NewGame()
	moves := g.GetLegalMoves()
	// Every returned move must either be executable (CanMove) or a castling move
	for _, move := range moves {
		assert.True(t, move.CanMove || move.IsCastle,
			"expected move %s to have CanMove=true or IsCastle=true", move.String())
	}
}

func TestGetLegalMoves_ContainsExpectedPawnMoves(t *testing.T) {
	g := NewGame()
	moves := g.GetLegalMoves()

	// Standard starting position should include all 16 pawn forward moves (e.g. e2-e4)
	expectedPawnMoves := []struct{ from, to string }{
		{"e2", "e4"}, {"e2", "e3"},
		{"d2", "d4"}, {"d2", "d3"},
	}

	for _, expected := range expectedPawnMoves {
		found := false
		for _, move := range moves {
			if move.From.Location.String() == expected.from && move.To.String() == expected.to {
				found = true
				break
			}
		}
		assert.True(t, found, "expected pawn move %s-%s to be in legal moves", expected.from, expected.to)
	}
}

func TestGetLegalMoves_ContainsKnightMoves(t *testing.T) {
	g := NewGame()
	moves := g.GetLegalMoves()

	// Both knights should have at least one valid move in the starting position
	knightMoveDestinations := map[string]bool{}
	for _, move := range moves {
		if move.From.Piece.Piece == game.Knight {
			knightMoveDestinations[move.To.String()] = true
		}
	}

	assert.True(t, knightMoveDestinations["c3"], "expected Nb1-c3 to be in legal moves")
	assert.True(t, knightMoveDestinations["a3"], "expected Nb1-a3 to be in legal moves")
	assert.True(t, knightMoveDestinations["f3"], "expected Ng1-f3 to be in legal moves")
	assert.True(t, knightMoveDestinations["h3"], "expected Ng1-h3 to be in legal moves")
}

func TestGetLegalMoves_ReturnsMovesForCurrentPlayer(t *testing.T) {
	g := NewGame()
	moves := g.GetLegalMoves()
	// All starting moves should belong to White pieces
	for _, move := range moves {
		assert.Equal(t, g.GetPosition().PlayerToMove, move.From.Piece.Color,
			"each legal move should be for the current player")
	}
}

func TestGetLegalMoves_AfterMove_SwitchesToOpponent(t *testing.T) {
	g := NewGame()

	ok, err := g.TrySanMove("e4")
	assert.True(t, ok)
	assert.NoError(t, err)

	moves := g.GetLegalMoves()
	assert.NotEmpty(t, moves)

	// After white plays e4, it is black's turn
	for _, move := range moves {
		assert.Equal(t, g.GetPosition().PlayerToMove, move.From.Piece.Color,
			"each legal move after e4 should be for Black")
	}
}


func TestNewGame_FenHasFullmoveNumberOne(t *testing.T) {
	g := NewGame()
	fenStr := fen.ToFenString(g.GetPosition())
	assert.Equal(t, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", fenStr,
		"FEN of new game should have fullmove number 1")
}

func TestGetResult_InProgressAtStart(t *testing.T) {
	g := NewGame()
	assert.Equal(t, game.InProgress, g.GetResult())
	assert.False(t, g.GetResult().IsDecided())
}

func TestGetResult_CheckmateIsDecided(t *testing.T) {
	// K+Q vs K where Black king (h8) is in checkmate:
	// White queen on g7 and king on h6 gives checkmate.
	board := game.NewChessBoard()
	board.SetSquare(game.ChessLocation{File: game.FileH, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})
	board.SetSquare(game.ChessLocation{File: game.FileG, Rank: game.Rank7}, game.ChessPiece{Piece: game.Queen, Color: game.WhitePiece})
	board.SetSquare(game.ChessLocation{File: game.FileH, Rank: game.Rank6}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})

	movement := game.NewChessMovement(&game.ChessPosition{
		Board:        board,
		PlayerToMove: game.BlackPiece,
		CastlingRights: map[game.ColorType]game.CastlingRights{
			game.WhitePiece: {},
			game.BlackPiece: {},
		},
		EnPassantSquare: game.ChessLocation{},
		HalfmoveClock:   0,
		FullmoveNumber:  1,
	})
	movement.Calculate()

	result := movement.Result
	assert.Equal(t, game.WhiteWins, result)
	assert.True(t, result.IsDecided())
	assert.False(t, result.IsDraw())
}

func TestGetResult_NoMoreMovesAfterGameOver(t *testing.T) {
	// After checkmate, TrySanMove must be rejected.
	board := game.NewChessBoard()
	board.SetSquare(game.ChessLocation{File: game.FileH, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})
	board.SetSquare(game.ChessLocation{File: game.FileG, Rank: game.Rank7}, game.ChessPiece{Piece: game.Queen, Color: game.WhitePiece})
	board.SetSquare(game.ChessLocation{File: game.FileH, Rank: game.Rank6}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})

	pos := &game.ChessPosition{
		Board:        board,
		PlayerToMove: game.BlackPiece,
		CastlingRights: map[game.ColorType]game.CastlingRights{
			game.WhitePiece: {},
			game.BlackPiece: {},
		},
		EnPassantSquare: game.ChessLocation{},
		HalfmoveClock:   0,
		FullmoveNumber:  1,
	}
	g := NewGameFromPosition(pos)
	ok, err := g.TrySanMove("Kg8")
	assert.False(t, ok)
	assert.Error(t, err)
}

func TestTrySanMove_PawnPromotion(t *testing.T) {
	tests := []struct {
		name          string
		boardSetup    func() *game.ChessPosition
		san           string
		expectOk      bool
		expectedPiece game.PieceType
		expectedColor game.ColorType
		expectedLoc   game.ChessLocation
	}{
		{
			name: "white pawn promotes to queen via e8=Q",
			boardSetup: func() *game.ChessPosition {
				board := game.NewChessBoard()
				board.SetSquare(game.ChessLocation{File: game.FileE, Rank: game.Rank7}, game.ChessPiece{Piece: game.Pawn, Color: game.WhitePiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank1}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})
				return &game.ChessPosition{
					Board:        board,
					PlayerToMove: game.WhitePiece,
					CastlingRights: map[game.ColorType]game.CastlingRights{
						game.WhitePiece: {},
						game.BlackPiece: {},
					},
				}
			},
			san:           "e8=Q",
			expectOk:      true,
			expectedPiece: game.Queen,
			expectedColor: game.WhitePiece,
			expectedLoc:   game.ChessLocation{File: game.FileE, Rank: game.Rank8},
		},
		{
			name: "white pawn promotes to knight via e8=N",
			boardSetup: func() *game.ChessPosition {
				board := game.NewChessBoard()
				board.SetSquare(game.ChessLocation{File: game.FileE, Rank: game.Rank7}, game.ChessPiece{Piece: game.Pawn, Color: game.WhitePiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank1}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})
				return &game.ChessPosition{
					Board:        board,
					PlayerToMove: game.WhitePiece,
					CastlingRights: map[game.ColorType]game.CastlingRights{
						game.WhitePiece: {},
						game.BlackPiece: {},
					},
				}
			},
			san:           "e8=N",
			expectOk:      true,
			expectedPiece: game.Knight,
			expectedColor: game.WhitePiece,
			expectedLoc:   game.ChessLocation{File: game.FileE, Rank: game.Rank8},
		},
		{
			name: "black pawn promotes to rook via e1=R",
			boardSetup: func() *game.ChessPosition {
				board := game.NewChessBoard()
				board.SetSquare(game.ChessLocation{File: game.FileE, Rank: game.Rank2}, game.ChessPiece{Piece: game.Pawn, Color: game.BlackPiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank1}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})
				board.SetSquare(game.ChessLocation{File: game.FileA, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})
				return &game.ChessPosition{
					Board:        board,
					PlayerToMove: game.BlackPiece,
					CastlingRights: map[game.ColorType]game.CastlingRights{
						game.WhitePiece: {},
						game.BlackPiece: {},
					},
				}
			},
			san:           "e1=R",
			expectOk:      true,
			expectedPiece: game.Rook,
			expectedColor: game.BlackPiece,
			expectedLoc:   game.ChessLocation{File: game.FileE, Rank: game.Rank1},
		},
	}

	for _, test := range tests {
		t.Run(test.name, func(t *testing.T) {
			g := NewGameFromPosition(test.boardSetup())
			ok, err := g.TrySanMove(test.san)
			assert.Equal(t, test.expectOk, ok)
			if test.expectOk {
				assert.NoError(t, err)
				square := g.GetPosition().Board.GetSquare(test.expectedLoc)
				assert.Equal(t, test.expectedPiece, square.Piece.Piece, "promoted piece type mismatch")
				assert.Equal(t, test.expectedColor, square.Piece.Color, "promoted piece color mismatch")
			}
		})
	}
}

// newGameFromFen is a test helper that creates a ChessGame from a FEN string.
func newGameFromFen(t *testing.T, fenString string) *ChessGame {
	t.Helper()
	pos, err := fen.ParseFen(fenString)
	require.NoError(t, err)
	return NewGameFromPosition(&pos)
}

func TestCastleKingSide_UpdatesPosition(t *testing.T) {
	// FEN with white able to castle kingside: king on e1, rook on h1, no pieces between.
	g := newGameFromFen(t, "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1")

	beforePos := g.GetPosition()
	ok, err := g.TrySanMove("O-O")
	assert.True(t, ok)
	assert.NoError(t, err)

	afterPos := g.GetPosition()
	// Position must have changed after castling.
	assert.NotEqual(t, beforePos, afterPos, "position should change after kingside castling")

	// White king should now be on g1.
	piece, ok := afterPos.Board.GetPiece(game.ChessLocation{File: game.FileG, Rank: game.Rank1})
	assert.True(t, ok)
	assert.Equal(t, game.King, piece.Piece)
	assert.Equal(t, game.WhitePiece, piece.Color)

	// White rook should now be on f1.
	piece, ok = afterPos.Board.GetPiece(game.ChessLocation{File: game.FileF, Rank: game.Rank1})
	assert.True(t, ok)
	assert.Equal(t, game.Rook, piece.Piece)
	assert.Equal(t, game.WhitePiece, piece.Color)
}

func TestCastleQueenSide_UpdatesPosition(t *testing.T) {
	// FEN with white able to castle queenside: king on e1, rook on a1, no pieces between.
	g := newGameFromFen(t, "r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1")

	beforePos := g.GetPosition()
	ok, err := g.TrySanMove("O-O-O")
	assert.True(t, ok)
	assert.NoError(t, err)

	afterPos := g.GetPosition()
	// Position must have changed after castling.
	assert.NotEqual(t, beforePos, afterPos, "position should change after queenside castling")

	// White king should now be on c1.
	piece, ok := afterPos.Board.GetPiece(game.ChessLocation{File: game.FileC, Rank: game.Rank1})
	assert.True(t, ok)
	assert.Equal(t, game.King, piece.Piece)
	assert.Equal(t, game.WhitePiece, piece.Color)

	// White rook should now be on d1.
	piece, ok = afterPos.Board.GetPiece(game.ChessLocation{File: game.FileD, Rank: game.Rank1})
	assert.True(t, ok)
	assert.Equal(t, game.Rook, piece.Piece)
	assert.Equal(t, game.WhitePiece, piece.Color)
}

func TestGetResult_DrawByThreefoldRepetition(t *testing.T) {
	g := NewGame()

	// Bounce both knights back and forth so positions repeat.
	// The starting position is recorded once at construction (count=1).
	// After 4 moves (Nc3, Nc6, Nb1, Nb8) we return to the start (count=2).
	// After 7 moves (…, Nc3, Nc6, Nb1) no position has reached count=3 yet.
	moves := []string{"Nc3", "Nc6", "Nb1", "Nb8", "Nc3", "Nc6", "Nb1"}
	for _, m := range moves {
		ok, err := g.TrySanMove(m)
		assert.True(t, ok, "move %s should succeed", m)
		assert.NoError(t, err)
	}
	assert.Equal(t, game.InProgress, g.GetResult(), "game should still be in progress after 7 moves")

	// 8th move returns to the starting position for the third time.
	ok, err := g.TrySanMove("Nb8")
	assert.True(t, ok)
	assert.NoError(t, err)
	assert.Equal(t, game.DrawRepetition, g.GetResult())
	assert.True(t, g.GetResult().IsDraw())
	assert.True(t, g.GetResult().IsDecided())
}

func TestGetResult_NoMoreMovesAfterDrawRepetition(t *testing.T) {
	g := NewGame()

	moves := []string{"Nc3", "Nc6", "Nb1", "Nb8", "Nc3", "Nc6", "Nb1", "Nb8"}
	for _, m := range moves {
		_, _ = g.TrySanMove(m)
	}

	ok, err := g.TrySanMove("e4")
	assert.False(t, ok)
	assert.Error(t, err)
}

func TestGetResult_DrawInsufficientMaterial(t *testing.T) {
	board := game.NewChessBoard()
	board.SetSquare(game.ChessLocation{File: game.FileE, Rank: game.Rank1}, game.ChessPiece{Piece: game.King, Color: game.WhitePiece})
	board.SetSquare(game.ChessLocation{File: game.FileE, Rank: game.Rank8}, game.ChessPiece{Piece: game.King, Color: game.BlackPiece})

	movement := game.NewChessMovement(&game.ChessPosition{
		Board:        board,
		PlayerToMove: game.WhitePiece,
		CastlingRights: map[game.ColorType]game.CastlingRights{
			game.WhitePiece: {},
			game.BlackPiece: {},
		},
		EnPassantSquare: game.ChessLocation{},
		HalfmoveClock:   0,
		FullmoveNumber:  1,
	})
	movement.Calculate()

	assert.Equal(t, game.DrawInsufficientMaterial, movement.Result)
	assert.True(t, movement.Result.IsDraw())
}
