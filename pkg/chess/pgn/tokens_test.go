package pgn

import (
	"github.com/stretchr/testify/assert"
	"strings"
	"testing"
)

func CreateObjUnderTest(value string) *PgnTokenReader {
	return NewPgnTokenReader(strings.NewReader(value))
}

var simpleTokenTests = []struct {
	inputString   string
	tokenType     PgnTokenType
	expectedValue string
}{
	{"12345", Integer, "12345"},
	{"S_Y-M+B#O=L:09", Symbol, "S_Y-M+B#O=L:09"},
	{"[", LeftBracket, "["},
	{"]", RightBracket, "]"},
	{"<", LeftAngle, "<"},
	{">", RightAngle, ">"},
	{"*", Astrix, "*"},
	{";comment string", CommentRestOfLine, "comment string"},
	{"{comment string}", CommentInLine, "comment string"},
	{"$12345", NumericAnnotationGlyph, "12345"},
	{".", Period, "."},
	{"\"string\"", String, "string"},
	{"%escape string", Escape, "escape string"},
	{"1/2-1/2", Symbol, "1/2-1/2"},
}

func TestSimpleTokenMatches(t *testing.T) {
	for _, tt := range simpleTokenTests {
		t.Run(tt.inputString, func(t *testing.T) {
			tokenReader := CreateObjUnderTest(tt.inputString)
			tokens, err := tokenReader.ReadTokens()

			assert.Nil(t, err)
			assert.Len(t, tokens, 1)
			assert.Equal(t, tt.tokenType, tokens[0].Type)
			assert.Equal(t, tt.expectedValue, tokens[0].Value)
			assert.Equal(t, 0, tokens[0].Position.Offset)
			assert.Equal(t, 0, tokens[0].Position.LineOffset)
			assert.Equal(t, 0, tokens[0].Position.Line)
		})
	}
}

var tagTokens = "[Name1 \"Value\"]\n[Name2 \"Value2\"]"

var tagTokensExpected = []PgnToken{
	{PgnTokenPosition{0, 0, 0, 0}, "[", LeftBracket},
	{PgnTokenPosition{1, 1, 0, 1}, "Name1", Symbol},
	{PgnTokenPosition{7, 7, 0, 7}, "Value", String},
	{PgnTokenPosition{14, 14, 0, 14}, "]", RightBracket},
	{PgnTokenPosition{16, 16, 1, 0}, "[", LeftBracket},
	{PgnTokenPosition{17, 17, 1, 1}, "Name2", Symbol},
	{PgnTokenPosition{23, 23, 1, 7}, "Value2", String},
	{PgnTokenPosition{31, 31, 1, 15}, "]", RightBracket},
}

func TestTagTokens(t *testing.T) {
	tokenReader := CreateObjUnderTest(tagTokens)
	tokens, err := tokenReader.ReadTokens()

	assert.Nil(t, err)
	assert.Len(t, tokens, 8)
	assert.Equal(t, tagTokensExpected, tokens)
}
