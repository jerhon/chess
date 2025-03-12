package position

import (
	"fmt"
	"github.com/jerhon/chess/board"
	"github.com/jerhon/chess/san"
)

type CastlingRights struct {
	KingSide  bool
	QueenSide bool
}

// ChessPosition represents a position in a chess game
type ChessPosition struct {
	Board           *board.ChessBoard
	PlayerToMove    board.ColorType
	CastlingRights  map[board.ColorType]CastlingRights
	EnPassantSquare board.ChessLocation
	HalfmoveClock   int
	FullmoveNumber  int

	calculator *ChessMoveCalculator
}

// GetMoveFromSan returns a chess move from a given San string
func (position *ChessPosition) GetMoveFromSan(sanString string) (ChessMove, error) {

	sanMove, err := san.ParseSan(sanString)
	if err != nil {
		return ChessMove{}, err
	}

	if position.calculator == nil {
		position.calculator = NewChessMoveCalculator(position)
		position.calculator.Calculate()
	}

	moves := position.calculator.GetMoves()

	candidateSanMoves := []ChessMove{}

	// find squares that we can move to
	for _, move := range moves {
		if sanMove.FromRank != board.NoRank && sanMove.FromRank != move.From.Rank {
			continue
		}

		if sanMove.FromFile != board.NoFile && sanMove.FromFile != move.From.File {
			continue
		}

		if sanMove.Piece != board.NoPiece && sanMove.Piece != move.Piece.Piece {
			continue
		}

		if move.To.Rank == sanMove.ToRank && move.To.File == sanMove.ToFile {
			candidateSanMoves = append(candidateSanMoves, move)
		}
	}

	if len(candidateSanMoves) == 0 {
		return ChessMove{}, fmt.Errorf("No moves found for san string: %s", sanString)
	} else if len(candidateSanMoves) == 1 {
		return candidateSanMoves[0], nil
	} else {
		return ChessMove{}, fmt.Errorf("Multiple moves found for san string: %s", sanString)
	}
}

// Move performs a chess move, does not take into consideration whether the move is valid
func (position *ChessPosition) Move(fromLocation board.ChessLocation, toLocation board.ChessLocation) ChessPosition {
	fromSquare := position.Board.GetSquare(fromLocation)
	toSquare := position.Board.GetSquare(toLocation)

	halfmoveClock := position.HalfmoveClock + 1
	if !toSquare.IsEmpty() {
		halfmoveClock = 0
	}
	if fromSquare.Piece.Piece == board.Pawn {
		halfmoveClock = 0
	}

	fullMoveNumber := position.FullmoveNumber
	if position.PlayerToMove == board.BlackPiece {
		fullMoveNumber++
	}

	castlingRights := map[board.ColorType]CastlingRights{}
	// update for castling rights
	if fromSquare.Piece.Piece == board.King {
		position.CastlingRights[position.PlayerToMove] = CastlingRights{}
	}
	if fromSquare.Piece.Piece == board.Rook {
		currentCastlingRights := position.CastlingRights[position.PlayerToMove]
		if position.PlayerToMove == board.BlackPiece {
			if fromLocation.File == board.FileE {
				castlingRights[position.PlayerToMove] = CastlingRights{QueenSide: currentCastlingRights.QueenSide, KingSide: false}
			}
			if fromLocation.File == board.FileA {
				castlingRights[position.PlayerToMove] = CastlingRights{QueenSide: false, KingSide: currentCastlingRights.KingSide}
			}
		}
	}

	enPassantTarget := board.ChessLocation{}
	if fromSquare.Piece.Piece == board.Pawn {
		if fromLocation.Rank == board.Rank2 && toLocation.Rank == board.Rank4 {
			enPassantTarget = board.ChessLocation{fromLocation.File, board.Rank3}
		} else if fromLocation.Rank == board.Rank7 && toLocation.Rank == board.Rank5 {
			enPassantTarget = board.ChessLocation{fromLocation.File, board.Rank6}
		}
	}

	playerToMove := board.BlackPiece
	if position.PlayerToMove == board.BlackPiece {
		playerToMove = board.WhitePiece
	}

	newBoard := position.Board.Clone()
	newBoard.SetSquare(fromLocation, board.ChessPiece{board.NoPiece, board.NoColor})
	newBoard.SetSquare(toLocation, fromSquare.Piece)

	// if this is an en passant move, need to remove the pawn
	if fromSquare.Piece.Piece == board.Pawn {
		if toLocation == position.EnPassantSquare {
			if position.PlayerToMove == board.BlackPiece {
				newBoard.SetSquare(board.ChessLocation{fromLocation.File, board.Rank7}, board.ChessPiece{board.NoPiece, board.NoColor})
			} else {
				newBoard.SetSquare(board.ChessLocation{fromLocation.File, board.Rank3}, board.ChessPiece{board.NoPiece, board.NoColor})
			}
			halfmoveClock = 0
		}
	}

	return ChessPosition{
		Board:           newBoard,
		PlayerToMove:    playerToMove,
		CastlingRights:  castlingRights,
		EnPassantSquare: enPassantTarget,
		HalfmoveClock:   halfmoveClock,
		FullmoveNumber:  fullMoveNumber,
	}
}
