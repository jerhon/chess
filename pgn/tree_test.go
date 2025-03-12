package pgn

import (
	"github.com/stretchr/testify/assert"
	"strings"
	"testing"
)

// This sample is taken from the PGN specification

var pgnSpecSample = `
[Event "F/S Return Match"] 
[Site "Belgrade, Serbia JUG"] 
[Date "1992.11.04"] 
[Round "29"] 
[White "Fischer, Robert J."]
[Black "Spassky, Boris V."] 
[Result "1/2-1/2"] 

1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O-O Be7 6. Re1 b5 7. Bb3 d6 8. c3
O-O 9. h3 Nb8 10. d4 Nbd7 11. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15.
Nb1 h6 16. Bh4 c5 17. dxe5 Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21.
Nc4 Nxc4 22. Bxc4 Nb6 23. Ne5 Rae8 24. Bxf7+ Rxf7 25. Nxf7 Rxe1+ 26. Qxe1 Kxf7
27. Qe3 Qg5 28. Qxg5 hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33.
f3 Bc8 34. Kf2 Bf5 35. Ra7 g6 36. Ra6+ Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5
40. Rd6 Kc5 41. Ra6 Nf2 42. g4 Bd3 43. Re6 1/2-1/2
`

func createPgnGameFromString(rawPgn string) (PgnGame, error) {
	tokenReader := NewPgnTokenReader(strings.NewReader(rawPgn))
	tokens, err := tokenReader.ReadTokens()
	if err != nil {
		return PgnGame{}, err
	}
	parser := NewPgnGameParser(tokens)
	game, err := parser.Parse()
	return game, err
}

var expectedTag = []PgnTag{
	{"Event", "F/S Return Match"},
	{"Site", "Belgrade, Serbia JUG"},
	{"Date", "1992.11.04"},
	{"Round", "29"},
	{"White", "Fischer, Robert J."},
	{"Black", "Spassky, Boris V."},
	{"Result", "1/2-1/2"},
}

func NewPgnElement(number string, move string) PgnElement {
	return PgnElement{number, move, "", nil}
}

var moves = []PgnElement{
	NewPgnElement("1.", "e4"),
	NewPgnElement("", "e5"),
	NewPgnElement("2.", "Nf3"),
	NewPgnElement("", "Nc6"),
	NewPgnElement("3.", "Bb5"),
	NewPgnElement("", "a6"),
	NewPgnElement("4.", "Ba4"),
	NewPgnElement("", "Nf6"),
	NewPgnElement("5.", "O-O"),
	NewPgnElement("", "Be7"),
	NewPgnElement("6.", "Re1"),
	NewPgnElement("", "b5"),
}

func TestTagParsing(t *testing.T) {

	game, err := createPgnGameFromString(pgnSpecSample)

	assert.Nil(t, err)
	assert.NotNil(t, game)
	assert.Len(t, game.TagSection, 7)
	assert.Equal(t, expectedTag, game.TagSection)

	// take only the number I have in the initial pgn
	actualMoves := game.MoveText[:len(moves)]
	assert.Equal(t, moves, actualMoves)

	assert.Equal(t, "1/2-1/2", game.Result)
}
