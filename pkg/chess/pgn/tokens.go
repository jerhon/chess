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
	// The rules of the token in the source string
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
func (p *PgnTokenReader) ReadTokens() (tokens []PgnToken, err error) {
	tokens = []PgnToken{}

	match, r := p.readRune()
	for match {
		newLine := p.lineOffset == 0
		startPosition := p.getPosition()

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
			match, r = p.readRune()
			continue
		} else if unicode.IsLetter(r) || unicode.IsDigit(r) {
			value := ""
			for isSymbolRune(r) && match {
				value += string(r)
				match, r = p.readRune()
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
			match, r = p.readRune()
			for match && (escape || r != '"') {
				if escape {
					value += string(r)
					escape = false
				} else {
					if r == '\\' {
						escape = true
					} else {
						value += string(r)
					}
				}
				match, r = p.readRune()
			}

			// advance past the final "
			match, r = p.readRune()

			token := PgnToken{startPosition, value, String}
			tokens = append(tokens, token)
		} else if r == '%' && newLine {
			value := ""
			match, r = p.readRune()
			for r != '\n' && r != '\r' && match {
				value += string(r)
				match, r = p.readRune()
			}
			token := PgnToken{startPosition, value, Escape}
			tokens = append(tokens, token)
		} else if r == '$' {
			value := ""
			match, r = p.readRune()
			for unicode.IsDigit(r) && match {
				value += string(r)
				match, r = p.readRune()
			}
			token := PgnToken{startPosition, value, NumericAnnotationGlyph}
			tokens = append(tokens, token)
		} else if r == ';' {
			value := ""
			match, r = p.readRune()
			for r != '\n' && r != '\r' && match {
				value += string(r)
				match, r = p.readRune()
			}
			token := PgnToken{startPosition, value, CommentRestOfLine}
			tokens = append(tokens, token)
		} else if r == '{' {
			value := ""
			match, r = p.readRune()
			for r != '}' && match {
				value += string(r)
				match, r = p.readRune()
			}
			// ignore ending }
			match, r = p.readRune()
			token := PgnToken{
				Position: startPosition,
				Value:    value,
				Type:     CommentInLine,
			}
			tokens = append(tokens, token)
		} else if unicode.IsSpace(r) {
			// ignore it
			match, r = p.readRune()
		} else {
			token := PgnToken{startPosition, string(r), Unknown}
			tokens = append(tokens, token)
			match, r = p.readRune()
		}
	}
	return
}

// getPosition gets the current rules the PgnTokenReader is at
func (p *PgnTokenReader) getPosition() PgnTokenPosition {
	return PgnTokenPosition{
		Offset:     p.offset,
		RuneOffset: p.runeOffset,
		Line:       p.line,
		LineOffset: p.lineOffset,
	}
}

// readRune reads a single rune from the PgnTokenReader and advances all offsets as necessary
func (p *PgnTokenReader) readRune() (bool, rune) {

	previousRune := p.rune
	previousRuneSize := p.runeSize

	r, runeSize, err := p.reader.ReadRune()
	if err != nil {
		return false, 0
	}

	if previousRune == '\n' {
		p.line++
		p.lineOffset = 0
	} else if previousRuneSize != 0 {
		p.lineOffset++
	}

	p.offset += previousRuneSize
	p.runeOffset++
	p.rune = r
	p.runeSize = runeSize

	return true, p.rune
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
