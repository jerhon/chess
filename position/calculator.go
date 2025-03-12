package position

import (
	"github.com/jerhon/chess/board"
	"iter"
)

// ChessMoveCalculator calculates the moves for a given chess position
type ChessMoveCalculator struct {
	Position *ChessPosition

	CandidateMoves map[board.ColorType][]ChessMove

	// Moves contains the list of valid moves for the current player
	Moves []ChessMove

	KingLocation map[board.ColorType]board.ChessLocation
	Check        map[board.ColorType]bool

	IsCheckmate bool
	IsStalemate bool
	Calculated  bool
}

func NewChessMoveCalculator(position *ChessPosition) *ChessMoveCalculator {
	return &ChessMoveCalculator{
		Position: position,
		CandidateMoves: map[board.ColorType][]ChessMove{
			board.WhitePiece: {},
			board.BlackPiece: {},
		},
		KingLocation: map[board.ColorType]board.ChessLocation{},
		Check: map[board.ColorType]bool{
			board.WhitePiece: false,
			board.BlackPiece: false,
		},
		IsCheckmate: false,
		IsStalemate: false,
		Calculated:  false,
	}
}

var allColors = []board.ColorType{board.WhitePiece, board.BlackPiece}

func oppositeColor(color board.ColorType) board.ColorType {
	if color == board.WhitePiece {
		return board.BlackPiece
	} else {
		return board.WhitePiece
	}
}

func (calculator *ChessMoveCalculator) Calculate() {
	if calculator.Calculated {
		return
	}

	calculator.calculateCandidateMoves()
	calculator.calculateCheck()
	calculator.calculateValidMoves()
	calculator.Calculated = true
}

func (calculator *ChessMoveCalculator) GetMoves() []ChessMove {
	calculator.Calculate()
	return calculator.Moves
}

func (calculator *ChessMoveCalculator) calculateCheck() {

	for _, color := range allColors {
		kingLocation := calculator.KingLocation[color]

		// check each of the opponent moves to see if they attack the king
		for _, move := range calculator.CandidateMoves[oppositeColor(color)] {
			if move.To == kingLocation {
				calculator.Check[color] = true
				break
			}
		}
	}

}

// CalculateValidMoves this will calculate any valid moves (moves that won't put the king in check)
func (calculator *ChessMoveCalculator) calculateValidMoves() {

	// King has special logic, it cannot step into check
	candidateMoves := calculator.CandidateMoves[calculator.Position.PlayerToMove]

	// TODO: this is pretty expensive, look at optimizing in the future
	finalMoves := []ChessMove{}
	for _, move := range candidateMoves {

		// cannot move into check
		candidatePosition := calculator.Position.Move(move.From, move.To)

		// Just need to calculate through check
		calculator = NewChessMoveCalculator(&candidatePosition)
		calculator.calculateCandidateMoves()
		calculator.calculateCheck()

		if !calculator.Check[calculator.Position.PlayerToMove] {
			finalMoves = append(finalMoves, move)
		}
	}

	calculator.Moves = finalMoves

	// if there are no moves, we're in checkmate or stalemate
	if len(finalMoves) == 0 {
		if calculator.Check[calculator.Position.PlayerToMove] {
			calculator.IsCheckmate = true
		} else {
			calculator.IsStalemate = true
		}
	}

}

func (calculator *ChessMoveCalculator) calculateCandidateMoves() {

	if calculator.Calculated {
		return
	}

	allMoves := map[board.ColorType][]ChessMove{

		board.WhitePiece: {},
		board.BlackPiece: {},
	}

	for square := range calculator.Position.Board.IterateSquares() {
		if square.IsEmpty() {
			continue
		}

		var moves []ChessMove

		switch square.Piece.Piece {
		case board.Pawn:
			moves = CalculatePawnMoves(calculator.Position, square.Location)
		case board.Rook:
			moves = CalculateRookMoves(calculator.Position, square.Location)
		case board.Knight:
			moves = CalculateKnightMoves(calculator.Position, square.Location)
		case board.Bishop:
			moves = CalculateBishopMoves(calculator.Position, square.Location)
		case board.Queen:
			moves = CalculateQueenMoves(calculator.Position, square.Location)
		case board.King:
			moves = CalculateKingMoves(calculator.Position, square.Location)
		default:
			continue
		}

		if square.Piece.Color == calculator.Position.PlayerToMove {
			if square.Piece.Piece == board.King {
				calculator.KingLocation[square.Piece.Color] = square.Location
			}
		}

		allMoves[square.Piece.Color] = append(allMoves[square.Piece.Color], moves...)
	}
}

type ChessMove struct {
	// Piece the piece to move
	Piece board.ChessPiece

	// From the location to move the piece from
	From board.ChessLocation

	// To the location to move the piece to
	To board.ChessLocation

	// IsCapture true if the move captures another piece
	IsCapture bool

	// IsCastle true if the move is a castle, if true, this will only represent the move of the king
	IsCastle bool
}

func CalculateRayMoves(position *ChessPosition, fromLocation board.ChessLocation, seq iter.Seq[iter.Seq[board.ChessLocation]]) []ChessMove {
	moves := []ChessMove{}

	fromPiece, _ := position.Board.GetPiece(fromLocation)
	for diagonal := range seq {
		for toLocation := range diagonal {
			toPiece, _ := position.Board.GetPiece(toLocation)
			newMoves, stop := tryAppendRayMove(moves, fromLocation, fromPiece, toLocation, toPiece)
			moves = newMoves
			if stop {
				break
			}
		}
	}

	return moves
}

func CalculatePawnMoves(position *ChessPosition, fromLocation board.ChessLocation) []ChessMove {
	moves := []ChessMove{}
	fromPiece, match := position.Board.GetPiece(fromLocation)

	if !match || fromPiece.Piece != board.Pawn {
		return moves
	}

	forwardOrBack := board.RankType(1)
	if fromPiece.Color == board.BlackPiece {
		forwardOrBack = board.RankType(-1)
	}

	// Pawn can move 1 square
	toLocation := board.ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File}
	_, match = position.Board.GetPiece(toLocation)
	if !match {
		moves = append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        toLocation,
			IsCapture: false,
		})
	}

	// If on starting location, can move 2 squares
	if isPawnOnStartingSquare(fromLocation, fromPiece.Color) {
		toLocation = board.ChessLocation{Rank: fromLocation.Rank + forwardOrBack*2, File: fromLocation.File}
		_, match = position.Board.GetPiece(toLocation)
		if !match {
			moves = append(moves, ChessMove{
				Piece:     fromPiece,
				From:      fromLocation,
				To:        toLocation,
				IsCapture: false,
			})
		}
	}

	// If there is a opponent piece to the diagonal add the capture move
	// or if the diagonal is the target of an en passant capture.
	toLocation = board.ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File + board.FileType(1)}
	toPiece, match := position.Board.GetPiece(toLocation)
	if (match && toPiece.Color != fromPiece.Color) || (toLocation == position.EnPassantSquare) {
		moves = append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        toLocation,
			IsCapture: true,
		})
	}

	toLocation = board.ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File - board.FileType(1)}
	toPiece, match = position.Board.GetPiece(toLocation)
	if (match && toPiece.Color != fromPiece.Color) || (toLocation == position.EnPassantSquare) {
		moves = append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        toLocation,
			IsCapture: true,
		})
	}

	return moves
}

func isPawnOnStartingSquare(pawnLocation board.ChessLocation, color board.ColorType) bool {
	return (pawnLocation.Rank == board.Rank2 && color == board.WhitePiece) || (pawnLocation.Rank == board.Rank7 && color == board.BlackPiece)
}

func CalculateBishopMoves(position *ChessPosition, fromLocation board.ChessLocation) []ChessMove {
	return CalculateRayMoves(position, fromLocation, iterateDiagonal(fromLocation))
}

func CalculateRookMoves(position *ChessPosition, fromLocation board.ChessLocation) []ChessMove {
	return CalculateRayMoves(position, fromLocation, iterateStraight(fromLocation))
}

func CalculateQueenMoves(position *ChessPosition, location board.ChessLocation) []ChessMove {
	diagonalMoves := CalculateBishopMoves(position, location)
	straightMoves := CalculateRookMoves(position, location)

	return append(diagonalMoves, straightMoves...)
}

func CalculateKnightMoves(position *ChessPosition, fromLocation board.ChessLocation) []ChessMove {

	fromPiece, match := position.Board.GetPiece(fromLocation)
	if !match || fromPiece.Piece != board.Knight {
		return []ChessMove{}
	}

	moves := []ChessMove{}

	candidateLocations := []struct {
		rankOffset int
		fileOffset int
	}{
		{2, 1},
		{2, -1},
		{-2, 1},
		{-2, -1},
		{1, 2},
		{-1, 2},
		{1, -2},
		{-1, -2},
	}

	for _, candidiateLocation := range candidateLocations {
		move, match := canAppendKnightMove(position, fromLocation, fromPiece, candidiateLocation.rankOffset, candidiateLocation.fileOffset)
		if match {
			moves = append(moves, move)
		}
	}

	return moves
}

func canAppendKnightMove(position *ChessPosition, fromLocation board.ChessLocation, fromPiece board.ChessPiece, rankOffset int, fileOffset int) (ChessMove, bool) {
	toPiece, match := position.Board.GetPiece(board.ChessLocation{File: fromLocation.File + board.FileType(fileOffset), Rank: fromLocation.Rank + board.RankType(rankOffset)})
	if match && toPiece.Color == fromPiece.Color {
		return ChessMove{}, false
	}

	return ChessMove{
		Piece:     fromPiece,
		From:      fromLocation,
		To:        board.ChessLocation{File: fromLocation.File + board.FileType(fileOffset), Rank: fromLocation.Rank + board.RankType(rankOffset)},
		IsCapture: toPiece.Piece != board.NoPiece,
	}, true
}

func CalculateKingMoves(position *ChessPosition, fromLocation board.ChessLocation) []ChessMove {
	moves := []ChessMove{}

	fromPiece, match := position.Board.GetPiece(fromLocation)
	if !match || fromPiece.Piece != board.King {
		return moves
	}

	for rank := fromLocation.Rank - 1; rank <= fromLocation.Rank+1; rank++ {
		for file := fromLocation.File - 1; file <= fromLocation.File+1; file++ {
			toLocation := board.ChessLocation{File: file, Rank: rank}

			if fromLocation == toLocation {
				continue
			}

			if piece, match := position.Board.GetPiece(fromLocation); match {
				if piece.Color != fromPiece.Color {
					moves = append(moves, ChessMove{
						Piece:     fromPiece,
						From:      fromLocation,
						To:        toLocation,
						IsCapture: true,
					})
				}
			} else {
				moves = append(moves, ChessMove{
					Piece:     fromPiece,
					From:      fromLocation,
					To:        toLocation,
					IsCapture: false,
				})
			}
		}
	}

	if position.CastlingRights[fromPiece.Color].KingSide {
		moves = append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        board.ChessLocation{File: board.FileG, Rank: fromLocation.Rank},
			IsCapture: false,
			IsCastle:  true,
		})
	}

	if position.CastlingRights[fromPiece.Color].QueenSide {
		moves = append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        board.ChessLocation{File: board.FileC, Rank: fromLocation.Rank},
			IsCapture: false,
			IsCastle:  true,
		})
	}

	return moves
}

func tryAppendRayMove(moves []ChessMove, fromLocation board.ChessLocation, fromPiece board.ChessPiece, toLocation board.ChessLocation, toPiece board.ChessPiece) ([]ChessMove, bool) {
	// No piece on the square, we are fine
	if toPiece.Piece == board.NoPiece {
		return append(moves, ChessMove{
			Piece:     fromPiece,
			From:      fromLocation,
			To:        toLocation,
			IsCapture: false,
		}), false
	}

	if toPiece.Color == fromPiece.Color {
		return moves, true
	}

	return append(moves, ChessMove{
		Piece:     fromPiece,
		From:      fromLocation,
		To:        toLocation,
		IsCapture: true,
	}), true
}

func iterateStraight(location board.ChessLocation) iter.Seq[iter.Seq[board.ChessLocation]] {
	return func(yield func(iter.Seq[board.ChessLocation]) bool) {
		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank + 1; rank <= board.Rank8; rank++ {
				yield2(board.ChessLocation{File: location.File, Rank: rank})
			}
			return
		})
		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank - 1; rank >= board.Rank1; rank-- {
				yield2(board.ChessLocation{File: location.File, Rank: rank})
			}
			return
		})
		return
	}
}

func iterateDiagonal(location board.ChessLocation) iter.Seq[iter.Seq[board.ChessLocation]] {
	return func(yield func(iter.Seq[board.ChessLocation]) bool) {
		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank + 1; rank <= board.Rank8; rank++ {
				for file := location.File + 1; file <= board.FileH; file++ {
					yield2(board.ChessLocation{File: file, Rank: rank})
				}
			}
			return
		})

		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank - 1; rank >= board.Rank1; rank-- {
				for file := location.File + 1; file <= board.FileH; file++ {
					yield2(board.ChessLocation{File: file, Rank: rank})
				}
			}
			return
		})

		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank + 1; rank <= board.Rank8; rank++ {
				for file := location.File - 1; file >= board.FileA; file-- {
					yield2(board.ChessLocation{File: file, Rank: rank})
				}
			}
			return
		})

		yield(func(yield2 func(board.ChessLocation) bool) {
			for rank := location.Rank - 1; rank >= board.Rank1; rank-- {
				for file := location.File - 1; file >= board.FileA; file-- {
					yield2(board.ChessLocation{File: file, Rank: rank})
				}
			}
			return
		})
	}
}
