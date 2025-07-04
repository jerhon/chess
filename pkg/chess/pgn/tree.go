package pgn

import "fmt"

// tree.go contains syntax tree parsing for PGN games

type PgnGame struct {
	TagSection []PgnTag

	MoveText []PgnElement

	Result string
}

type PgnTag struct {
	Name  string
	Value string
}

type PgnElementSequence struct {
	Elements []PgnElement
}

type PgnElement struct {
	MoveNumberIndicator    string
	SanMove                string
	NumericAnnotationGlyph string
	RecursiveAnnotation    []PgnElement
}

type PgnGameParser struct {
	tokens            []PgnToken
	idx               int
	parsingAttributes bool
}

func NewPgnGameParser(tokens []PgnToken) *PgnGameParser {
	return &PgnGameParser{tokens, 0, true}
}

func (this *PgnGameParser) peekToken() (match bool, token *PgnToken) {

	// We want to ignore comments when we are parsing the PGN for evaluation
	for this.idx < len(this.tokens) {
		currentToken := this.tokens[this.idx]
		if currentToken.Type == CommentInLine || currentToken.Type == CommentRestOfLine || currentToken.Type == Escape {
			this.idx++
		} else {
			break
		}
	}

	if this.idx < len(this.tokens) {
		return true, &this.tokens[this.idx]
	}
	return false, nil
}

// advanceToken advances the game parser by one token
func (this *PgnGameParser) advanceToken() (bool, *PgnToken) {
	this.idx++
	if this.idx < len(this.tokens) {
		return true, &this.tokens[this.idx]
	}
	return false, nil
}

// expectToken will retrieve one token and return false if the token of that type is not found, it will return the token and advance the parser
func (this *PgnGameParser) expectToken(expectedTokenType PgnTokenType) (bool, *PgnToken) {
	match, token := this.peekToken()
	if !match || token.Type != expectedTokenType {
		return false, token
	}
	this.advanceToken()
	return true, token
}

func generateErrorText(position PgnTokenPosition, errorText string) string {
	return fmt.Sprintf("[%d:%d] %s", position.Line, position.LineOffset, errorText)
}

func (this *PgnGameParser) Parse() (PgnGame, error) {
	tags := []PgnTag{}
	elements := []PgnElement{}
	result := ""
	parseErrors := []string{}

	match, _ := this.peekToken()
	for match {
		lastIdx := this.idx
		if this.parsingAttributes {
			leftBracketMatch, _ := this.expectToken(LeftBracket)
			if leftBracketMatch {
				nameMatch, nameToken := this.expectToken(Symbol)
				if !nameMatch {
					parseErrors = append(parseErrors, generateErrorText(nameToken.Position, "Expected tag name after '['"))
					continue
				}

				valueMatch, valueToken := this.expectToken(String)
				if !valueMatch {
					parseErrors = append(parseErrors, generateErrorText(valueToken.Position, "Expected tag value after tag name"))
					continue
				}

				endBracketMatch, _ := this.expectToken(RightBracket)
				if !endBracketMatch {
					parseErrors = append(parseErrors, generateErrorText(valueToken.Position, "Expected ']' after tag value"))
					continue
				}

				tag := PgnTag{nameToken.Value, valueToken.Value}
				tags = append(tags, tag)
			} else {
				this.parsingAttributes = false
				continue
			}
		} else {
			// ignore the move text until we get the tag parsing done

			moveNumberText := ""
			// parsing move text
			numberMatch, numberToken := this.expectToken(Integer)
			if numberMatch {

				// if the first is an integer, then it's a move number
				periods := ""
				periodMatch, _ := this.expectToken(Period)
				for periodMatch {
					periods += "."
					periodMatch, _ = this.expectToken(Period)
				}

				moveNumberText = numberToken.Value + periods
			}

			sanText := ""
			symbolMatch, symbolToken := this.expectToken(Symbol)
			if symbolMatch {
				if isGameResult(symbolToken.Value) {
					result = symbolToken.Value
					break
				} else {
					sanText = symbolToken.Value
				}
			}

			if !numberMatch && !symbolMatch {
				parseErrors = append(parseErrors, generateErrorText(symbolToken.Position, "Expected move number or move text.  Advancing to next PGN token."))
				this.advanceToken()
				continue
			}

			if numberMatch && !symbolMatch {
				parseErrors = append(parseErrors, generateErrorText(symbolToken.Position, "Expected move text after move number."))
				continue
			}

			numericAnnotationGlyph := ""
			numericAnnotationMatch, numericAnnotationToken := this.expectToken(NumericAnnotationGlyph)
			if numericAnnotationMatch {
				numericAnnotationGlyph = numericAnnotationToken.Value
			}

			element := PgnElement{moveNumberText, sanText, numericAnnotationGlyph, nil}
			elements = append(elements, element)
		}
		match, _ = this.peekToken()
		if lastIdx == this.idx {
			parseErrors = append(parseErrors, generateErrorText(this.tokens[this.idx].Position, "Internal parser error."))
		}
	}

	var err error = nil
	if (parseErrors != nil) && (len(parseErrors) > 0) {
		err = fmt.Errorf("One or more errors occurred parsing the PGN: %s", parseErrors)
	}

	return PgnGame{tags, elements, result}, err
}

func isGameResult(value string) bool {
	return value == "0-1" || value == "1-0" || value == "1/2-1/2" || value == "*"
}
