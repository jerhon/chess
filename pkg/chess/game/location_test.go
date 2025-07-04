package game

import (
	"github.com/stretchr/testify/assert"
	"strconv"
	"testing"
)

func TestFileType_String(t *testing.T) {
	tests := []struct {
		File FileType
		Want string
	}{
		{
			File: FileA,
			Want: "a",
		},
		{
			File: FileH,
			Want: "h",
		},
		{
			File: FileD,
			Want: "d",
		},
	}

	for _, tt := range tests {
		t.Run(tt.Want, func(t *testing.T) {
			assert.Equal(t, tt.Want, tt.File.String())
		})
	}
}

func TestFileType_ToIndex(t *testing.T) {
	tests := []struct {
		File FileType
		Want int
	}{
		{
			File: FileA,
			Want: 0,
		},
		{
			File: FileH,
			Want: 7,
		},
		{
			File: FileD,
			Want: 3,
		},
	}

	for _, tt := range tests {
		t.Run(strconv.Itoa(tt.Want), func(t *testing.T) {
			assert.Equal(t, tt.Want, tt.File.ToIndex())
		})
	}
}

func TestRankType_ToIndex(t *testing.T) {
	tests := []struct {
		Rank RankType
		Want int
	}{
		{
			Rank: Rank1,
			Want: 0,
		},
		{
			Rank: Rank8,
			Want: 7,
		},
		{
			Rank: Rank3,
			Want: 2,
		},
	}

	for _, tt := range tests {
		t.Run(strconv.Itoa(tt.Want), func(t *testing.T) {
			assert.Equal(t, tt.Want, tt.Rank.ToIndex())
		})
	}
}

func TestRankType_String(t *testing.T) {
	tests := []struct {
		Rank RankType
		Want string
	}{
		{
			Rank: Rank1,
			Want: "1",
		},
		{
			Rank: Rank8,
			Want: "8",
		},
		{
			Rank: Rank3,
			Want: "3",
		},
	}

	for _, tt := range tests {
		t.Run(tt.Want, func(t *testing.T) {
			assert.Equal(t, tt.Want, tt.Rank.String())
		})
	}
}

func TestChessLocation_String(t *testing.T) {
	tests := []struct {
		Location ChessLocation
		Want     string
	}{
		{
			Location: ChessLocation{
				File: FileA,
				Rank: Rank1,
			},
			Want: "a1",
		},
		{
			Location: ChessLocation{
				File: FileH,
				Rank: Rank8,
			},
			Want: "h8",
		},
		{
			Location: ChessLocation{
				File: FileD,
				Rank: Rank3,
			},
			Want: "d3",
		},
		{
			Location: ChessLocation{
				File: FileA,
				Rank: Rank8,
			},
			Want: "a8",
		},
	}

	for _, tt := range tests {
		t.Run(tt.Want, func(t *testing.T) {
			assert.Equal(t, tt.Want, tt.Location.String())
		})
	}
}
