package pgn

// https://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm

import (
	"strings"
	"unicode"
)

// PgnTokenType represents a single Token from a PGN file.
type PgnTokenType int

const (
	LeftBracket            PgnTokenType = 0
	RightBracket           PgnTokenType = 10
	Integer                PgnTokenType = 20
	String                 PgnTokenType = 30
	Period                 PgnTokenType = 40
	LeftParen              PgnTokenType = 50
	RightParen             PgnTokenType = 60
	LeftAngle              PgnTokenType = 70
	RightAngle             PgnTokenType = 80
	NumericAnnotationGlyph PgnTokenType = 90
	Symbol                 PgnTokenType = 100
	Escape                 PgnTokenType = 110
	CommentInLine          PgnTokenType = 120
	CommentRestOfLine      PgnTokenType = 130
	Unknown                PgnTokenType = 140
	Astrix                 PgnTokenType = 150
)

// PgnToken is a token in a PGN file.
type PgnToken struct {
	// The position of the token in the source string
	Position PgnTokenPosition
	// the value of the token
	Value string
	// the type of the token
	Type PgnTokenType
}

// PgnTokenPosition provides positional information for the start of a token
type PgnTokenPosition struct {
	Offset     int
	RuneOffset int
	Line       int
	LineOffset int
}

// PgnTokenReader encapsulates several fields to keep track of reading tokens
type PgnTokenReader struct {
	reader     *strings.Reader
	line       int
	lineOffset int
	offset     int
	runeOffset int
	rune       rune
	runeSize   int
}

// NewPgnTokenReader creates a new token reader for the given string reader
func NewPgnTokenReader(reader *strings.Reader) *PgnTokenReader {
	return &PgnTokenReader{
		reader:     reader,
		line:       0,
		lineOffset: 0,
		offset:     0,
		runeOffset: -1,
		rune:       0,
		runeSize:   0,
	}
}

// singleCharacterTokens contains a listing of PGN tokens which are single runes
var singleCharacterTokens = []struct {
	value     rune
	tokenType PgnTokenType
}{
	{'[', LeftBracket},
	{']', RightBracket},
	{'.', Period},
	{'(', LeftParen},
	{')', RightParen},
	{'<', LeftAngle},
	{'>', RightAngle},
	{'*', Astrix},
}

// ReadTokens reads all the PGN tokens from a PgnTokenReader
func (this *PgnTokenReader) ReadTokens() (tokens []PgnToken, err error) {
	tokens = []PgnToken{}

	match, r := this.readRune()
	for match {
		newLine := this.lineOffset == 0
		startPosition := this.getPosition()

		singleCharMatch := false
		for _, singleCharToken := range singleCharacterTokens {
			if r == singleCharToken.value {
				token := PgnToken{startPosition, string(r), singleCharToken.tokenType}
				tokens = append(tokens, token)
				singleCharMatch = true
				break
			}
		}

		if singleCharMatch {
			match, r = this.readRune()
			continue
		} else if unicode.IsLetter(r) || unicode.IsDigit(r) {
			value := ""
			for isSymbolRune(r) && match {
				value += string(r)
				match, r = this.readRune()
			}

			if isIntegerToken(value) {
				token := PgnToken{startPosition, value, Integer}
				tokens = append(tokens, token)
			} else {
				token := PgnToken{startPosition, value, Symbol}
				tokens = append(tokens, token)
			}
		} else if r == '"' {
			escape := false
			value := ""
			match, r = this.readRune()
			for match && (escape || r != '"') {
				if escape {
					if r != '\\' && r != '"' {
						// TODO add a parser error
					}
					value += string(r)
					escape = false
				} else {
					if r == '\\' {
						escape = true
					} else {
						value += string(r)
					}
				}
				match, r = this.readRune()
			}

			// advance past the final "
			match, r = this.readRune()

			token := PgnToken{startPosition, value, String}
			tokens = append(tokens, token)
		} else if r == '%' && newLine {
			value := ""
			match, r = this.readRune()
			for r != '\n' && r != '\r' && match {
				value += string(r)
				match, r = this.readRune()
			}
			token := PgnToken{startPosition, value, Escape}
			tokens = append(tokens, token)
		} else if r == '$' {
			value := ""
			match, r = this.readRune()
			for unicode.IsDigit(r) && match {
				value += string(r)
				match, r = this.readRune()
			}
			token := PgnToken{startPosition, value, NumericAnnotationGlyph}
			tokens = append(tokens, token)
		} else if r == ';' {
			value := ""
			match, r = this.readRune()
			for r != '\n' && r != '\r' && match {
				value += string(r)
				match, r = this.readRune()
			}
			token := PgnToken{startPosition, value, CommentRestOfLine}
			tokens = append(tokens, token)
		} else if r == '{' {
			value := ""
			match, r = this.readRune()
			for r != '}' && match {
				value += string(r)
				match, r = this.readRune()
			}
			// ignore ending }
			match, r = this.readRune()
			token := PgnToken{
				Position: startPosition,
				Value:    value,
				Type:     CommentInLine,
			}
			tokens = append(tokens, token)
		} else if unicode.IsSpace(r) {
			// ignore it
			match, r = this.readRune()
		} else {
			token := PgnToken{startPosition, string(r), Unknown}
			tokens = append(tokens, token)
			match, r = this.readRune()
		}
	}
	return
}

// getPosition gets the current position the PgnTokenReader is at
func (this *PgnTokenReader) getPosition() PgnTokenPosition {
	return PgnTokenPosition{
		Offset:     this.offset,
		RuneOffset: this.runeOffset,
		Line:       this.line,
		LineOffset: this.lineOffset,
	}
}

// readRune reads a single rune from the PgnTokenReader and advances all offsets as necessary
func (this *PgnTokenReader) readRune() (bool, rune) {

	previousRune := this.rune
	previousRuneSize := this.runeSize

	r, runeSize, err := this.reader.ReadRune()
	if err != nil {
		return false, 0
	}

	if previousRune == '\n' {
		this.line++
		this.lineOffset = 0
	} else if previousRuneSize != 0 {
		this.lineOffset++
	}

	this.offset += previousRuneSize
	this.runeOffset++
	this.rune = r
	this.runeSize = runeSize

	return true, this.rune
}

func isSymbolRune(r rune) bool {
	return unicode.IsLetter(r) || unicode.IsDigit(r) || r == '_' || r == '-' || r == '+' || r == '#' || r == '=' || r == ':' || r == '/'
}

func isIntegerToken(value string) bool {
	for _, r := range value {
		if !unicode.IsDigit(r) {
			return false
		}
	}
	return true
}
