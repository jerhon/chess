package chess

type PieceType rune

const (
	NoPiece PieceType = iota
	Pawn    PieceType = 'P'
	Knight  PieceType = 'N'
	Bishop  PieceType = 'B'
	Rook    PieceType = 'R'
	Queen   PieceType = 'Q'
	King    PieceType = 'K'
)

type ColorType rune

const (
	NoColor    ColorType = iota
	WhitePiece           = 'w'
	BlackPiece           = 'b'
)

type FileType rune

const (
	NoFile FileType = iota
	FileA  FileType = 'a'
	FileB  FileType = 'b'
	FileC  FileType = 'c'
	FileD  FileType = 'd'
	FileE  FileType = 'e'
	FileF  FileType = 'f'
	FileG  FileType = 'g'
	FileH  FileType = 'h'
)

type RankType rune

const (
	NoRank RankType = iota
	Rank1  RankType = '1'
	Rank2  RankType = '2'
	Rank3  RankType = '3'
	Rank4  RankType = '4'
	Rank5  RankType = '5'
	Rank6  RankType = '6'
	Rank7  RankType = '7'
	Rank8  RankType = '8'
)

type Game struct {
	board               ChessBoard
	blackCastlingRights CastlingRights
	whiteCastlingRights CastlingRights
	enPassantSquare     string
	halfmoveClock       int
	fullmoveNumber      int
	isCheck             bool
	isCheckmate         bool
	isStalemate         bool
}

type CastlingRights struct {
	KingSide  bool
	QueenSide bool
}

// ChessPosition represents a position in a chess game
type ChessPosition struct {
	Board               *ChessBoard
	PlayerToMove        ColorType
	BlackCastlingRights CastlingRights
	WhiteCastlingRights CastlingRights
	EnPassantSquare     ChessLocation
	HalfmoveClock       int
	FullmoveNumber      int
}

func (file FileType) String() string {
	return string(file)
}

func (rank RankType) String() string {
	return string(rank)
}
