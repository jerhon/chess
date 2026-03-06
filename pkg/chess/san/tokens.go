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

func (tr *SanTokenReader) readRune() (rune, error) {
	lastSize := tr.runeSize

	r, size, err := tr.reader.ReadRune()

	if err != nil {
		tr.err = err
		tr.rune = 0
		return 0, err
	}

	if lastSize != 0 {
		tr.runeOffset++
		tr.offset += lastSize
	}

	tr.runeSize = size
	tr.rune = r

	return r, nil
}

func (tr *SanTokenReader) peekRune() (rune, error) {
	if tr.err != nil {
		return 0, tr.err
	}
	if tr.runeSize == 0 {
		_, err := tr.readRune()
		if err != nil {
			return 0, err
		}
	}
	return tr.rune, nil
}

func (tr *SanTokenReader) getPosition() SanTokenPosition {
	return SanTokenPosition{
		RuneOffset: tr.runeOffset,
		Offset:     tr.offset,
	}
}

func (tr *SanTokenReader) expectAndAdvanceRune(expected rune) bool {
	r, err := tr.peekRune()
	if err != nil {
		return false
	}
	if r == expected {
		_, _ = tr.readRune()
		return true
	}
	return false
}

func (tr *SanTokenReader) ReadTokens() ([]SanToken, error) {

	parseErrors := []string{}
	tokens := []SanToken{}
	var r rune
	_, err := tr.peekRune()
	for err == nil {
		r, err = tr.peekRune()
		if r == ' ' {
			r, err = tr.readRune()
		}
		if err != nil {
			if len(parseErrors) > 0 {
				return tokens, fmt.Errorf("one or more parse errors occurred: %s", parseErrors)
			}
			return tokens, nil
		}
		tokenType := None
		startPosition := tr.getPosition()
		if r >= 'a' && r <= 'h' {
			// if an e is followed by a . followed by a p, it's en passant
			if r == 'e' {
				_, err = tr.readRune()
				if tr.expectAndAdvanceRune('.') {
					if !tr.expectAndAdvanceRune('p') {
						parseErrors = append(parseErrors, "Expected 'p' after 'e.'")
						continue
					}
					if !tr.expectAndAdvanceRune('.') {
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
			_, err = tr.readRune()
			continue
		}

		if r == '+' {
			r, err = tr.readRune()
			var value string
			if r == '+' {
				tokenType = CheckmateToken
				value = "++"
				_, err = tr.readRune()
			} else {
				tokenType = CheckToken
				value = "+"
			}
			tokens = append(tokens, SanToken{tokenType, value, startPosition})
			continue
		}

		return tokens, fmt.Errorf("unexpected character: %s", string(r))
	}

	return tokens, nil
}

func ParseSanTokens(san string) ([]SanToken, error) {
	reader := strings.NewReader(san)
	return NewSanTokenReader(reader).ReadTokens()
}
