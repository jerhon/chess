package game

// ChessPosition represents a rules in a chess game
type ChessPosition struct {
	Board           *ChessBoard
	PlayerToMove    ColorType
	CastlingRights  map[ColorType]CastlingRights
	EnPassantSquare ChessLocation
	HalfmoveClock   int
	FullmoveNumber  int
}

// CastlingRights represents the castling rights for a player
type CastlingRights struct {
	// KingSide is true if the player can castle king side
	KingSide bool
	// QueenSide is true if the player can castle queen side
	QueenSide bool
}

func (position *ChessPosition) CastleKingside() *ChessPosition {

	rank := Rank1
	if position.PlayerToMove == BlackPiece {
		rank = Rank8
	}

	kingSquare := position.Board.GetSquare(ChessLocation{FileE, rank})
	rookSquare := position.Board.GetSquare(ChessLocation{FileH, rank})
	newKingSquare := position.Board.GetSquare(ChessLocation{FileG, rank})
	newRookSquare := position.Board.GetSquare(ChessLocation{FileF, rank})

	// Create a new board
	newBoard := position.Board.Clone()

	// Update the new board
	newBoard.SetSquare(newKingSquare.Location, kingSquare.Piece)
	newBoard.SetSquare(newRookSquare.Location, rookSquare.Piece)
	newBoard.SetSquare(kingSquare.Location, ChessPiece{NoPiece, NoColor})
	newBoard.SetSquare(rookSquare.Location, ChessPiece{NoPiece, NoColor})

	// Update castling rights
	castlingRights := map[ColorType]CastlingRights{}
	for color, rights := range position.CastlingRights {
		if color == position.PlayerToMove {
			// After castling, the player loses all castling rights
			castlingRights[color] = CastlingRights{}
		} else {
			// Other player's castling rights remain unchanged
			castlingRights[color] = rights
		}
	}

	// Switch player to move
	playerToMove := BlackPiece
	if position.PlayerToMove == BlackPiece {
		playerToMove = WhitePiece
	}

	// Update fullmove number if black just moved
	fullMoveNumber := position.FullmoveNumber
	if position.PlayerToMove == BlackPiece {
		fullMoveNumber++
	}

	// Return a new position
	return &ChessPosition{
		Board:           newBoard,
		PlayerToMove:    playerToMove,
		CastlingRights:  castlingRights,
		EnPassantSquare: ChessLocation{}, // No en passant target after castling
		HalfmoveClock:   position.HalfmoveClock + 1, // Increment half-move clock
		FullmoveNumber:  fullMoveNumber,
	}
}

// Move performs a chess move, does not take into consideration whether the move is valid
func (position *ChessPosition) Move(fromLocation ChessLocation, toLocation ChessLocation) *ChessPosition {
	fromSquare := position.Board.GetSquare(fromLocation)
	toSquare := position.Board.GetSquare(toLocation)

	halfmoveClock := position.HalfmoveClock + 1
	if !toSquare.IsEmpty() {
		halfmoveClock = 0
	}
	if fromSquare.Piece.Piece == Pawn {
		halfmoveClock = 0
	}

	fullMoveNumber := position.FullmoveNumber
	if position.PlayerToMove == BlackPiece {
		fullMoveNumber++
	}

	castlingRights := map[ColorType]CastlingRights{}
	// update for castling rights
	if fromSquare.Piece.Piece == King {
		position.CastlingRights[position.PlayerToMove] = CastlingRights{}
	}
	if fromSquare.Piece.Piece == Rook {
		currentCastlingRights := position.CastlingRights[position.PlayerToMove]
		if position.PlayerToMove == BlackPiece {
			if fromLocation.File == FileE {
				castlingRights[position.PlayerToMove] = CastlingRights{QueenSide: currentCastlingRights.QueenSide, KingSide: false}
			}
			if fromLocation.File == FileA {
				castlingRights[position.PlayerToMove] = CastlingRights{QueenSide: false, KingSide: currentCastlingRights.KingSide}
			}
		}
	}

	enPassantTarget := ChessLocation{}
	if fromSquare.Piece.Piece == Pawn {
		if fromLocation.Rank == Rank2 && toLocation.Rank == Rank4 {
			enPassantTarget = ChessLocation{fromLocation.File, Rank3}
		} else if fromLocation.Rank == Rank7 && toLocation.Rank == Rank5 {
			enPassantTarget = ChessLocation{fromLocation.File, Rank6}
		}
	}

	playerToMove := BlackPiece
	if position.PlayerToMove == BlackPiece {
		playerToMove = WhitePiece
	}

	newBoard := position.Board.Clone()
	newBoard.SetSquare(fromLocation, ChessPiece{NoPiece, NoColor})
	newBoard.SetSquare(toLocation, fromSquare.Piece)

	// if this is an en passant move, need to remove the pawn
	if fromSquare.Piece.Piece == Pawn {
		if toLocation == position.EnPassantSquare {
			if position.PlayerToMove == BlackPiece {
				newBoard.SetSquare(ChessLocation{fromLocation.File, Rank7}, ChessPiece{NoPiece, NoColor})
			} else {
				newBoard.SetSquare(ChessLocation{fromLocation.File, Rank3}, ChessPiece{NoPiece, NoColor})
			}
			halfmoveClock = 0
		}
	}

	return &ChessPosition{
		Board:           newBoard,
		PlayerToMove:    playerToMove,
		CastlingRights:  castlingRights,
		EnPassantSquare: enPassantTarget,
		HalfmoveClock:   halfmoveClock,
		FullmoveNumber:  fullMoveNumber,
	}
}
