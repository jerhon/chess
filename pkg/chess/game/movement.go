package game

import (
	"iter"
	"strings"
)

// ChessMovement calculates the moves
type ChessMovement struct {
	Position *ChessPosition

	CandidateMoves map[ColorType][]ChessMove

	// Moves contain the list of valid moves for the current player
	Moves []ChessMove

	KingLocation map[ColorType]ChessLocation
	Check        map[ColorType]bool

	CanCastle CastlingRights

	IsCheckmate bool
	IsStalemate bool
	Calculated  bool
}

func NewChessMovement(position *ChessPosition) *ChessMovement {
	return &ChessMovement{
		Position: position,
		CandidateMoves: map[ColorType][]ChessMove{
			WhitePiece: {},
			BlackPiece: {},
		},
		KingLocation: map[ColorType]ChessLocation{},
		Check: map[ColorType]bool{
			WhitePiece: false,
			BlackPiece: false,
		},
		IsCheckmate: false,
		IsStalemate: false,
		Calculated:  false,
	}
}

func (calculator *ChessMovement) Calculate() {
	if calculator.Calculated {
		return
	}

	calculator.calculateCandidateMoves()
	calculator.calculateCheck()
	calculator.calculateValidMoves()
	calculator.Calculated = true
}

func (calculator *ChessMovement) GetMoves() []ChessMove {
	calculator.Calculate()
	return calculator.Moves
}

func (calculator *ChessMovement) calculateCheck() {

	for _, color := range AllColors {
		kingLocation := calculator.KingLocation[color]

		// check each of the opponent moves to see if they attack the king
		for _, move := range calculator.CandidateMoves[color.OppositeColor()] {
			if move.To == kingLocation {
				calculator.Check[color] = true
				break
			}
		}
	}

}

func (calculator *ChessMovement) calculateCanCastle() {
	// Ensure the method is only calculated once

	playerColor := calculator.Position.PlayerToMove
	castlingRights := calculator.Position.CastlingRights[playerColor]

	kingLocation := calculator.KingLocation[playerColor]
	if kingLocation == (ChessLocation{}) {
		return
	}

	// Check King-side castling
	if castlingRights.KingSide {
		kingSideClear := true
		for file := kingLocation.File + 1; file < FileG; file++ {
			if !calculator.Position.Board.GetSquare(ChessLocation{File: file, Rank: kingLocation.Rank}).IsEmpty() {
				kingSideClear = false
				break
			}
		}

		if kingSideClear && !calculator.isPathUnderAttack(kingLocation, FileG, playerColor) {
			calculator.CanCastle.KingSide = true
		}
	}

	// Check Queen-side castling
	if castlingRights.QueenSide {
		queenSideClear := true
		for file := kingLocation.File - 1; file > FileC; file-- {
			if !calculator.Position.Board.GetSquare(ChessLocation{File: file, Rank: kingLocation.Rank}).IsEmpty() {
				queenSideClear = false
				break
			}
		}

		if queenSideClear && !calculator.isPathUnderAttack(kingLocation, FileC, playerColor) {
			calculator.CanCastle.QueenSide = true
		}
	}
}

// Helper method to check if the path for castling is under attack
func (calculator *ChessMovement) isPathUnderAttack(kingLocation ChessLocation, targetFile FileType, playerColor ColorType) bool {
	for file := kingLocation.File; file != targetFile; {
		if file < targetFile {
			file++
		} else {
			file--
		}

		location := ChessLocation{File: file, Rank: kingLocation.Rank}
		for _, move := range calculator.CandidateMoves[playerColor.OppositeColor()] {
			if move.To == location {
				return true
			}
		}
	}
	return false
}

// CalculateValidMoves this will calculate any valid moves (moves that won't put the king in check)
func (calculator *ChessMovement) calculateValidMoves() {

	// King has special logic, it cannot step into check
	candidateMoves := calculator.CandidateMoves[calculator.Position.PlayerToMove]

	// TODO: this is pretty expensive, look at optimizing in the future
	finalMoves := []ChessMove{}
	for _, move := range candidateMoves {

		// cannot move into check
		candidatePosition := calculator.Position.Move(move.From.Location, move.To)

		// Just need to calculate through check
		checkCalculator := NewChessMovement(candidatePosition)
		checkCalculator.calculateCandidateMoves()
		checkCalculator.calculateCheck()

		if !checkCalculator.Check[calculator.Position.PlayerToMove] {
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

func (calculator *ChessMovement) calculateCandidateMoves() {

	if calculator.Calculated {
		return
	}

	allMoves := map[ColorType][]ChessMove{
		WhitePiece: {},
		BlackPiece: {},
	}

	for square := range calculator.Position.Board.IterateSquares() {
		if square.IsEmpty() {
			continue
		}

		if square.Piece.Color == calculator.Position.PlayerToMove {
			if square.Piece.Piece == King {
				calculator.KingLocation[square.Piece.Color] = square.Location
			}
		}

		moves := CalculateMoves(calculator.Position, square.Location)

		allMoves[square.Piece.Color] = append(allMoves[square.Piece.Color], moves...)
	}

	calculator.CandidateMoves = allMoves
}

type ChessMove struct {
	// From is the square the piece is moving from
	From ChessSquare

	// To the location to move the piece to
	To ChessLocation

	// CanMove true if the move is a normal move (can move to the location) this could be false if it is a pawn move where there is nothing to capture
	CanMove bool

	// CanCapture true if the move can capture a piece
	CanCapture bool

	// IsCastle true if the move is a castle, if true, this will only represent the move of the king
	IsCastle bool
}

func (move ChessMove) String() string {
	builder := strings.Builder{}

	builder.WriteString(move.From.String())

	if move.CanCapture {
		builder.WriteString("x")
	}

	builder.WriteString(move.To.String())

	if move.IsCastle {
		builder.WriteString("-")
	}

	return builder.String()

}

func CalculateMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {

	square := position.Board.GetSquare(fromLocation)

	switch square.Piece.Piece {
	case Pawn:
		return calculatePawnMoves(position, fromLocation)
	case Rook:
		return calculateRookMoves(position, fromLocation)
	case Knight:
		return calculateKnightMoves(position, fromLocation)
	case Bishop:
		return calculateBishopMoves(position, fromLocation)
	case Queen:
		return calculateQueenMoves(position, fromLocation)
	case King:
		return calculateKingMoves(position, fromLocation)
	default:
		return []ChessMove{}
	}
}

func CalculateRayMoves(position *ChessPosition, fromLocation ChessLocation, seq []iter.Seq[ChessLocation]) []ChessMove {
	moves := []ChessMove{}

	fromSquare := position.Board.GetSquare(fromLocation)
	for _, diagonal := range seq {
		for toLocation := range diagonal {
			toSquare := position.Board.GetSquare(toLocation)
			newMoves, stop := tryAppendRayMove(moves, fromSquare, toSquare)
			moves = newMoves
			if stop {
				break
			}
		}
	}

	return moves
}

func calculatePawnMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {
	moves := []ChessMove{}
	fromSquare := position.Board.GetSquare(fromLocation)
	fromPiece, match := position.Board.GetPiece(fromLocation)

	if !match || fromPiece.Piece != Pawn {
		return moves
	}

	forwardOrBack := RankType(1)
	if fromPiece.Color == BlackPiece {
		forwardOrBack = RankType(-1)
	}

	// Pawn can move 1 square
	toLocation := ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File}
	_, match = position.Board.GetPiece(toLocation)
	if !match {
		moves = append(moves, ChessMove{
			From:       fromSquare,
			To:         toLocation,
			CanMove:    true,
			CanCapture: false,
		})

		// If the pawn is on a starting location, it can move 2 squares
		if isPawnOnStartingSquare(fromLocation, fromPiece.Color) {
			toLocation = ChessLocation{Rank: fromLocation.Rank + forwardOrBack*2, File: fromLocation.File}
			_, match = position.Board.GetPiece(toLocation)
			if !match {
				moves = append(moves, ChessMove{
					From:       fromSquare,
					To:         toLocation,
					CanMove:    true,
					CanCapture: false,
				})
			}
		}
	}

	// If there is a opponent piece to the diagonal add the capture move
	// or if the diagonal is the target of an en passant capture.
	toLocation = ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File + FileType(1)}
	toPiece, match := position.Board.GetPiece(toLocation)
	isCapture := (match && toPiece.Color != fromPiece.Color) || (toLocation == position.EnPassantSquare)
	moves = append(moves, ChessMove{
		From:       fromSquare,
		To:         toLocation,
		CanCapture: true,
		CanMove:    isCapture,
	})

	toLocation = ChessLocation{Rank: fromLocation.Rank + forwardOrBack, File: fromLocation.File - FileType(1)}
	toPiece, match = position.Board.GetPiece(toLocation)
	isCapture = (match && toPiece.Color != fromPiece.Color) || (toLocation == position.EnPassantSquare)
	moves = append(moves, ChessMove{
		From:       fromSquare,
		To:         toLocation,
		CanCapture: true,
		CanMove:    isCapture,
	})

	return moves
}

func isPawnOnStartingSquare(pawnLocation ChessLocation, color ColorType) bool {
	return (pawnLocation.Rank == Rank2 && color == WhitePiece) || (pawnLocation.Rank == Rank7 && color == BlackPiece)
}

func calculateBishopMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {
	return CalculateRayMoves(position, fromLocation, iterateDiagonal(fromLocation))
}

func calculateRookMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {
	return CalculateRayMoves(position, fromLocation, iterateStraight(fromLocation))
}

func calculateQueenMoves(position *ChessPosition, location ChessLocation) []ChessMove {
	diagonalMoves := calculateBishopMoves(position, location)
	straightMoves := calculateRookMoves(position, location)

	return append(diagonalMoves, straightMoves...)
}

func calculateKnightMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {

	fromSquare := position.Board.GetSquare(fromLocation)
	if fromSquare.Piece.Piece != Knight {
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
		move, match := canAppendKnightMove(position, fromSquare, candidiateLocation.rankOffset, candidiateLocation.fileOffset)
		if match {
			moves = append(moves, move)
		}
	}

	return moves
}

func canAppendKnightMove(position *ChessPosition, fromSquare ChessSquare, rankOffset int, fileOffset int) (ChessMove, bool) {
	fromLocation := fromSquare.Location
	fromPiece := fromSquare.Piece
	toPiece, match := position.Board.GetPiece(ChessLocation{File: fromLocation.File + FileType(fileOffset), Rank: fromLocation.Rank + RankType(rankOffset)})
	if match && toPiece.Color == fromPiece.Color {
		return ChessMove{}, false
	}

	return ChessMove{
		From:       fromSquare,
		To:         ChessLocation{File: fromLocation.File + FileType(fileOffset), Rank: fromLocation.Rank + RankType(rankOffset)},
		CanCapture: true,
		CanMove:    true,
	}, true
}

func calculateKingMoves(position *ChessPosition, fromLocation ChessLocation) []ChessMove {
	moves := []ChessMove{}

	fromPiece, match := position.Board.GetPiece(fromLocation)
	if !match || fromPiece.Piece != King {
		return moves
	}

	for rank := fromLocation.Rank - 1; rank <= fromLocation.Rank+1; rank++ {
		for file := fromLocation.File - 1; file <= fromLocation.File+1; file++ {
			toLocation := ChessLocation{File: file, Rank: rank}

			if fromLocation == toLocation {
				continue
			}

			if _, match := position.Board.GetPiece(fromLocation); match {
				moves = append(moves, ChessMove{
					From:       position.Board.GetSquare(fromLocation),
					To:         toLocation,
					CanMove:    true,
					CanCapture: true,
				})
			}
		}
	}

	if position.CastlingRights[fromPiece.Color].KingSide {
		moves = append(moves, ChessMove{
			From:       position.Board.GetSquare(fromLocation),
			To:         ChessLocation{File: FileG, Rank: fromLocation.Rank},
			CanCapture: false,
			CanMove:    false,
			IsCastle:   true,
		})
	}

	if position.CastlingRights[fromPiece.Color].QueenSide {
		moves = append(moves, ChessMove{
			From:       position.Board.GetSquare(fromLocation),
			To:         ChessLocation{File: FileC, Rank: fromLocation.Rank},
			CanMove:    false,
			CanCapture: false,
			IsCastle:   true,
		})
	}

	return moves
}

func tryAppendRayMove(moves []ChessMove, fromSquare ChessSquare, toSquare ChessSquare) ([]ChessMove, bool) {
	// No piece on the square, we are fine
	if toSquare.Piece.Piece == NoPiece {
		return append(moves, ChessMove{
			From:       fromSquare,
			To:         toSquare.Location,
			CanMove:    true,
			CanCapture: false,
		}), false
	}

	if toSquare.Piece.Color == fromSquare.Piece.Color {
		return moves, true
	}

	return append(moves, ChessMove{
		From:       fromSquare,
		To:         toSquare.Location,
		CanMove:    true,
		CanCapture: true,
	}), true
}

func iterateStraight(location ChessLocation) []iter.Seq[ChessLocation] {
	offsets := []offsetsStruct{
		{0, 1},
		{0, -1},
		{1, 0},
		{-1, 0},
	}

	return iterateOffsets(location, offsets)
}

func iterateDiagonal(location ChessLocation) []iter.Seq[ChessLocation] {
	offsets := []offsetsStruct{
		{1, 1},
		{1, -1},
		{-1, 1},
		{-1, -1},
	}

	return iterateOffsets(location, offsets)
}

type offsetsStruct struct {
	fileOffset int
	rankOffset int
}

func iterateOffsets(location ChessLocation, offsets []offsetsStruct) []iter.Seq[ChessLocation] {

	iters := []iter.Seq[ChessLocation]{}

	for _, offset := range offsets {
		offsetIter := func(yield func(ChessLocation) bool) {
			nextLocation := location.AddOffset(offset.fileOffset, offset.rankOffset)
			for nextLocation.IsOnBoard() {
				if !yield(nextLocation) {
					break
				}
				nextLocation = nextLocation.AddOffset(offset.fileOffset, offset.rankOffset)
			}
		}
		iters = append(iters, offsetIter)
	}

	return iters
}
