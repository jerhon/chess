package san

import (
	"fmt"
	"strings"
)

type SanToken struct {
	Type     SanTokenType
	Value    string
	Position SanTokenPosition
}

type SanTokenType int

const (
	None           SanTokenType = iota
	FileToken      SanTokenType = 1
	RankToken      SanTokenType = 2
	CaptureToken   SanTokenType = 3
	CheckToken     SanTokenType = 4
	CheckmateToken SanTokenType = 5
	PieceToken     SanTokenType = 6
	PromotionToken SanTokenType = 7
	EnPassantToken SanTokenType = 9
)

type SanTokenPosition struct {
	RuneOffset int
	Offset     int
}

type SanTokenReader struct {
	reader     *strings.Reader
	offset     int
	runeOffset int
	runeSize   int
	rune       rune
	err        error
}

func NewSanTokenReader(reader *strings.Reader) *SanTokenReader {
	return &SanTokenReader{
		reader:     reader,
		offset:     0,
		runeOffset: 0,
		runeSize:   0,
		rune:       0,
	}
}

func (this *SanTokenReader) readRune() (rune, error) {
	lastSize := this.runeSize

	r, size, err := this.reader.ReadRune()

	if err != nil {
		this.err = err
		this.rune = 0
		return 0, err
	}

	if lastSize != 0 {
		this.runeOffset++
		this.offset += lastSize
	}

	this.runeSize = size
	this.rune = r

	return r, nil
}

func (this *SanTokenReader) peekRune() (rune, error) {
	if this.err != nil {
		return 0, this.err
	}
	if this.runeSize == 0 {
		_, err := this.readRune()
		if err != nil {
			return 0, err
		}
	}
	return this.rune, nil
}

func (this *SanTokenReader) getPosition() SanTokenPosition {
	return SanTokenPosition{
		RuneOffset: this.runeOffset,
		Offset:     this.offset,
	}
}

func (this *SanTokenReader) expectAndAdvanceRune(expected rune) bool {
	r, err := this.peekRune()
	if err != nil {
		return false
	}
	if r == expected {
		_, _ = this.readRune()
		return true
	}
	return false
}

func (this *SanTokenReader) ReadTokens() ([]SanToken, error) {

	parseErrors := []string{}
	tokens := []SanToken{}
	r, err := this.peekRune()
	for err == nil {
		r, err = this.peekRune()
		if r == ' ' {
			r, err = this.readRune()
		}
		if err != nil {
			if len(parseErrors) > 0 {
				return tokens, fmt.Errorf("one or more parse errors occurred: %s", parseErrors)
			}
			return tokens, nil
		}
		tokenType := None
		startPosition := this.getPosition()
		if r >= 'a' && r <= 'h' {
			// if an e is followed by a . followed by a p, it's en passant
			if r == 'e' {
				r, err = this.readRune()
				if this.expectAndAdvanceRune('.') {
					if !this.expectAndAdvanceRune('p') {
						parseErrors = append(parseErrors, "Expected 'p' after 'e.'")
						continue
					}
					if !this.expectAndAdvanceRune('.') {
						parseErrors = append(parseErrors, "Expected '.' after 'e.p'")
						continue
					}

					tokenType = EnPassantToken
					tokens = append(tokens, SanToken{tokenType, "e.p.", startPosition})
					continue
				} else {
					tokenType = FileToken
					tokens = append(tokens, SanToken{tokenType, "e", startPosition})
					continue
				}
			}
			tokenType = FileToken
		} else if r >= '1' && r <= '8' {
			tokenType = RankToken
		} else if r == 'x' {
			tokenType = CaptureToken
		} else if r == 'P' || r == 'R' || r == 'N' || r == 'B' || r == 'Q' || r == 'K' {
			tokenType = PieceToken
		} else if r == '=' {
			tokenType = PromotionToken
		} else if r == '#' {
			tokenType = CheckmateToken
		}
		if tokenType != None {
			tokens = append(tokens, SanToken{tokenType, string(r), startPosition})
			r, err = this.readRune()
			continue
		}

		if r == '+' {
			r, err = this.readRune()
			var value string
			if r == '+' {
				tokenType = CheckmateToken
				value = "++"
				r, err = this.readRune()
			} else {
				tokenType = CheckToken
				value = "+"
			}
			tokens = append(tokens, SanToken{tokenType, value, startPosition})
			continue
		}

		parseErrors = append(parseErrors, "Unexpected character: "+string(r))
		break
	}

	return tokens, nil
}

func ParseSanTokens(san string) ([]SanToken, error) {
	reader := strings.NewReader(san)
	return NewSanTokenReader(reader).ReadTokens()
}
