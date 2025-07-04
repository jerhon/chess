package game

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

type ChessLocation struct {
	File FileType
	Rank RankType
}

func (file FileType) String() string {
	return string(file)
}

func (rank RankType) String() string {
	return string(rank)
}

func (location ChessLocation) String() string {
	return location.File.String() + location.Rank.String()
}

func (file FileType) ToIndex() int {
	return int(file) - int(FileA)
}

func (rank RankType) ToIndex() int {
	return int(rank) - int(Rank1)
}

func (location ChessLocation) AddOffset(fileOffset int, rankOffset int) ChessLocation {
	return ChessLocation{
		File: location.File + FileType(fileOffset),
		Rank: location.Rank + RankType(rankOffset),
	}
}

func (location ChessLocation) IsOnBoard() bool {
	return location.File >= FileA && location.File <= FileH && location.Rank >= Rank1 && location.Rank <= Rank8
}

func ParseChessLocation(location string) ChessLocation {
	file := FileType(location[0])
	rank := RankType(location[1])

	return ChessLocation{
		File: file,
		Rank: rank,
	}
}
