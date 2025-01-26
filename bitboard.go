package chess

import (
	"github.com/jerhon/chess/san"
	"strings"
)

// Bitboard represents a piece on the board and it's
type Bitboard struct {
	piece ChessPiece

	file san.FileType

	rank san.RankType

	bits uint64

	// pseudoMoveBits are all positions a chess piece could move to
	pseudoMoveBits uint64

	// psuedoAttackBits are all positions a chess piece could attack
	pseudoAttackBits uint64
}

func NewBitboard(piece ChessPiece) *Bitboard {
	return &Bitboard{
		piece: piece,
	}
}

// Move will move the piece to the new file and rank
func (board *Bitboard) Move(toFile san.FileType, toRank san.RankType) {
	board.bits = 0
	bit := uint64(1) << uint64((toFile.ToIndex()*8)+toRank.ToIndex())
	board.bits = bit
	board.file = toFile
	board.rank = toRank

	board.updatePseudoMoveBitboard()
}

func (board *Bitboard) updatePseudoMoveBitboard() {
	var resultBits uint64
	if board.piece.Piece == san.Rook {
		resultBits = 0
		var rankBits uint64 = 1 << board.rank.ToIndex()
		for i := 0; i < 8; i++ {
			resultBits |= rankBits << (i * 8)
		}
		resultBits |= uint64(0xFF) << (board.file.ToIndex() * 8)
		board.pseudoMoveBits = resultBits
		board.pseudoAttackBits = resultBits
	}
}

func (board Bitboard) String() string {
	builder := strings.Builder{}
	builder.WriteRune(rune(board.piece.Piece))
	builder.WriteRune(rune(board.file))
	builder.WriteRune(rune(board.rank))
	builder.WriteRune('\n')

	for file := 7; file >= 0; file-- {
		builder.WriteString(rankToBitRow(board.bits, file))
		builder.WriteString(" ")
		builder.WriteString(rankToBitRow(board.pseudoMoveBits, file))
		builder.WriteString(" ")
		builder.WriteString(rankToBitRow(board.pseudoAttackBits, file))
		builder.WriteRune('\n')
	}
	return builder.String()
}

func rankToBitRow(bits uint64, rankIdx int) string {
	bitString := [8]rune{}
	startOffset := rankIdx * 8
	for i := 0; i < 8; i++ {
		var mask uint64 = (uint64(1) << (startOffset + i))
		if bits&mask != 0 {
			bitString[i] = '1'
		} else {
			bitString[i] = '0'
		}
	}

	return string(bitString[:])
}
