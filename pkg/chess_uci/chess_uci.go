package chess_uci

import (
	"fmt"
	"strconv"
	"strings"
)

// UciCommand is the common interface for all parsed UCI commands.
// Use a type switch on the returned value from ParseUciCommand to
// work with specific command structs.
//
// For reference: https://backscattering.de/chess/uci/
// Supported commands:
//   - uci
//   - debug [on|off]
//   - isready
//   - setoption name <name> [value <value>]
//   - ucinewgame
//   - position [startpos | fen <FEN(6 fields)>] [moves <move> ...]
//   - go [searchmoves ...] [ponder] [wtime <ms>] [btime <ms>] [winc <ms>] [binc <ms>]
//        [movestogo <N>] [depth <N>] [nodes <N>] [mate <N>] [movetime <ms>] [infinite]
//   - stop
//   - ponderhit
//   - quit
//
// Notes:
// - setoption name/value can contain spaces. This parser collects all tokens for name until
//   the next keyword ("value") or end of input, likewise for value.
// - FEN is collected as the next 6 space-separated fields after the "fen" keyword.
// - Unrecognized commands return an error.
// - Unknown/extra parameters for known commands are tolerated (ignored) unless ambiguous.

type UciCommand interface {
	Name() string
}

// uci

type CmdUci struct{}

func (CmdUci) Name() string { return "uci" }

// debug [on|off]

type CmdDebug struct{ On bool }

func (CmdDebug) Name() string { return "debug" }

// isready

type CmdIsReady struct{}

func (CmdIsReady) Name() string { return "isready" }

// setoption name <name> [value <value>]

type CmdSetOption struct {
	OptionName string
	Value      *string
}

func (CmdSetOption) Name() string { return "setoption" }

// ucinewgame

type CmdUciNewGame struct{}

func (CmdUciNewGame) Name() string { return "ucinewgame" }

// position [startpos | fen <FEN(6 fields)>] [moves <move> ...]

type CmdPosition struct {
	StartPos bool
	FEN      string   // empty if StartPos is true
	Moves    []string // optional sequence of UCI moves
}

func (CmdPosition) Name() string { return "position" }

// go ... parameters

type CmdGo struct {
	SearchMoves []string
	Ponder      bool
	WTime       *int // milliseconds
	BTime       *int
	WInc        *int
	BInc        *int
	MovesToGo   *int
	Depth       *int
	Nodes       *int
	Mate        *int
	MoveTime    *int
	Infinite    bool
}

func (CmdGo) Name() string { return "go" }

// stop

type CmdStop struct{}

func (CmdStop) Name() string { return "stop" }

// ponderhit

type CmdPonderHit struct{}

func (CmdPonderHit) Name() string { return "ponderhit" }

// quit

type CmdQuit struct{}

func (CmdQuit) Name() string { return "quit" }

// ParseUciCommand parses a single UCI command line and returns a concrete command type.
// It is case-insensitive for the command keyword but otherwise follows UCI token rules.
func ParseUciCommand(input string) (UciCommand, error) {
	s := strings.TrimSpace(input)
	if s == "" {
		return nil, fmt.Errorf("empty input")
	}
	tokens := fieldsKeepEmpty(s)
	if len(tokens) == 0 {
		return nil, fmt.Errorf("no tokens")
	}
	cmd := strings.ToLower(tokens[0])
	switch cmd {
	case "uci":
		return CmdUci{}, nil
	case "debug":
		return parseDebug(tokens)
	case "isready":
		return CmdIsReady{}, nil
	case "setoption":
		return parseSetOption(tokens)
	case "ucinewgame":
		return CmdUciNewGame{}, nil
	case "position":
		return parsePosition(tokens)
	case "go":
		return parseGo(tokens)
	case "stop":
		return CmdStop{}, nil
	case "ponderhit":
		return CmdPonderHit{}, nil
	case "quit":
		return CmdQuit{}, nil
	default:
		return nil, fmt.Errorf("unknown command: %s", tokens[0])
	}
}

func parseDebug(tokens []string) (UciCommand, error) {
	// debug [on|off] — default to on if omitted
	if len(tokens) == 1 {
		return CmdDebug{On: true}, nil
	}
	arg := strings.ToLower(tokens[1])
	if arg == "on" {
		return CmdDebug{On: true}, nil
	}
	if arg == "off" {
		return CmdDebug{On: false}, nil
	}
	return nil, fmt.Errorf("debug expects 'on' or 'off', got %q", tokens[1])
}

func parseSetOption(tokens []string) (UciCommand, error) {
	// setoption name <name> [value <value>]
	// name/value may contain spaces
	if len(tokens) < 2 {
		return nil, fmt.Errorf("setoption requires at least 'name'")
	}
	i := 1
	if i >= len(tokens) || strings.ToLower(tokens[i]) != "name" {
		return nil, fmt.Errorf("setoption: expected 'name'")
	}
	i++
	nameStart := i
	for i < len(tokens) && strings.ToLower(tokens[i]) != "value" {
		i++
	}
	if nameStart >= len(tokens) {
		return nil, fmt.Errorf("setoption: missing option name")
	}
	name := strings.Join(tokens[nameStart:i], " ")
	var valPtr *string
	if i < len(tokens) && strings.ToLower(tokens[i]) == "value" {
		i++
		value := strings.Join(tokens[i:], " ")
		valPtr = &value
	}
	return CmdSetOption{OptionName: name, Value: valPtr}, nil
}

func parsePosition(tokens []string) (UciCommand, error) {
	// position [startpos | fen <FEN(6 fields)>] [moves <move> ...]
	if len(tokens) < 2 {
		return nil, fmt.Errorf("position requires arguments")
	}
	i := 1
	res := CmdPosition{}
	if strings.ToLower(tokens[i]) == "startpos" {
		res.StartPos = true
		i++
	} else if strings.ToLower(tokens[i]) == "fen" {
		i++
		// collect 6 fields of FEN
		fenFields := []string{}
		for i < len(tokens) && len(fenFields) < 6 {
			fenFields = append(fenFields, tokens[i])
			i++
		}
		if len(fenFields) < 6 {
			return nil, fmt.Errorf("position fen: expected 6 fields, got %d", len(fenFields))
		}
		res.FEN = strings.Join(fenFields, " ")
	} else {
		return nil, fmt.Errorf("position: expected 'startpos' or 'fen'")
	}

	// optional moves
	if i < len(tokens) && strings.ToLower(tokens[i]) == "moves" {
		i++
		for i < len(tokens) {
			mv := tokens[i]
			if mv != "" {
				res.Moves = append(res.Moves, mv)
			}
			i++
		}
	}
	return res, nil
}

func parseGo(tokens []string) (UciCommand, error) {
	// go [searchmoves <move> ...] [ponder] [wtime <ms>] [btime <ms>] [winc <ms>] [binc <ms>]
	//    [movestogo <N>] [depth <N>] [nodes <N>] [mate <N>] [movetime <ms>] [infinite]
	g := CmdGo{}
	i := 1
	for i < len(tokens) {
		key := strings.ToLower(tokens[i])
		switch key {
		case "searchmoves":
			i++
			for i < len(tokens) {
				low := strings.ToLower(tokens[i])
				if isGoCommandKeyword(low) { // next keyword reached
					break
				}
				g.SearchMoves = append(g.SearchMoves, tokens[i])
				i++
			}
			continue
		case "ponder":
			g.Ponder = true
			i++
			continue
		case "wtime":
			g.WTime = parseNextIntPtr(tokens, &i)
			continue
		case "btime":
			g.BTime = parseNextIntPtr(tokens, &i)
			continue
		case "winc":
			g.WInc = parseNextIntPtr(tokens, &i)
			continue
		case "binc":
			g.BInc = parseNextIntPtr(tokens, &i)
			continue
		case "movestogo":
			g.MovesToGo = parseNextIntPtr(tokens, &i)
			continue
		case "depth":
			g.Depth = parseNextIntPtr(tokens, &i)
			continue
		case "nodes":
			g.Nodes = parseNextIntPtr(tokens, &i)
			continue
		case "mate":
			g.Mate = parseNextIntPtr(tokens, &i)
			continue
		case "movetime":
			g.MoveTime = parseNextIntPtr(tokens, &i)
			continue
		case "infinite":
			g.Infinite = true
			i++
			continue
		default:
			// Unknown token — ignore gracefully to be robust, but step forward to avoid infinite loop
			i++
		}
	}
	return g, nil
}

// helpers

func fieldsKeepEmpty(s string) []string {
	// UCI is simple: it's fine to split on any ASCII whitespace and drop empty parts
	return strings.Fields(s)
}

func isGoCommandKeyword(tok string) bool {
	switch tok {
	case "searchmoves", "ponder", "wtime", "btime", "winc", "binc", "movestogo", "depth", "nodes", "mate", "movetime", "infinite":
		return true
	default:
		return false
	}
}

func parseNextIntPtr(tokens []string, i *int) *int {
	// tokens[*i] is the key; we need tokens[*i+1] as the value
	if *i+1 >= len(tokens) {
		*i = *i + 1
		return nil
	}
	// advance to value
	*i = *i + 1
	vStr := tokens[*i]
	*i = *i + 1
	if n, err := strconv.Atoi(vStr); err == nil {
		return &n
	}
	return nil
}
