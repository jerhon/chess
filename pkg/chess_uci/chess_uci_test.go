package chess_uci

import (
	"reflect"
	"testing"
)

func TestParseUciCommand_Uci(t *testing.T) {
	cmd, err := ParseUciCommand("uci")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if _, ok := cmd.(CmdUci); !ok {
		t.Fatalf("expected CmdUci, got %T", cmd)
	}

	cmd, err = ParseUciCommand("  UCI  ")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if _, ok := cmd.(CmdUci); !ok {
		t.Fatalf("expected CmdUci (case-insensitive), got %T", cmd)
	}
}

func TestParseUciCommand_Debug(t *testing.T) {
	tests := []struct {
		in      string
		want    CmdDebug
		wantErr bool
	}{
		{"debug", CmdDebug{On: true}, false},
		{"debug on", CmdDebug{On: true}, false},
		{"debug off", CmdDebug{On: false}, false},
		{"debug maybe", CmdDebug{}, true},
	}
	for _, tt := range tests {
		t.Run(tt.in, func(t *testing.T) {
			cmd, err := ParseUciCommand(tt.in)
			if tt.wantErr {
				if err == nil {
					t.Fatalf("expected error, got nil")
				}
				return
			}
			if err != nil {
				t.Fatalf("unexpected error: %v", err)
			}
			got, ok := cmd.(CmdDebug)
			if !ok {
				t.Fatalf("expected CmdDebug, got %T", cmd)
			}
			if got != tt.want {
				t.Fatalf("unexpected CmdDebug: %+v, want %+v", got, tt.want)
			}
		})
	}
}

func TestParseUciCommand_IsReady(t *testing.T) {
	cmd, err := ParseUciCommand("isready")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if _, ok := cmd.(CmdIsReady); !ok {
		t.Fatalf("expected CmdIsReady, got %T", cmd)
	}
}

func TestParseUciCommand_SetOption(t *testing.T) {
	tests := []struct {
		name     string
		in       string
		wantName string
		wantVal  *string
		wantErr  bool
	}{
		{"name only", "setoption name Hash", "Hash", nil, false},
		{"name+value", "setoption name Threads value 4", "Threads", strPtr("4"), false},
		{"multiword name", "setoption name Multi PV", "Multi PV", nil, false},
		{"multiword value", "setoption name EvalFile value nnue net large", "EvalFile", strPtr("nnue net large"), false},
		{"missing name keyword", "setoption value x", "", nil, true},
		{"missing option name", "setoption name", "", nil, true},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			cmd, err := ParseUciCommand(tt.in)
			if tt.wantErr {
				if err == nil {
					t.Fatalf("expected error, got nil")
				}
				return
			}
			if err != nil {
				t.Fatalf("unexpected error: %v", err)
			}
			got, ok := cmd.(CmdSetOption)
			if !ok {
				t.Fatalf("expected CmdSetOption, got %T", cmd)
			}
			if got.OptionName != tt.wantName {
				t.Fatalf("OptionName = %q, want %q", got.OptionName, tt.wantName)
			}
			if !strPtrEqual(got.Value, tt.wantVal) {
				t.Fatalf("Value = %v, want %v", got.Value, tt.wantVal)
			}
		})
	}
}

func TestParseUciCommand_UciNewGame(t *testing.T) {
	cmd, err := ParseUciCommand("ucinewgame")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if _, ok := cmd.(CmdUciNewGame); !ok {
		t.Fatalf("expected CmdUciNewGame, got %T", cmd)
	}
}

func TestParseUciCommand_Position(t *testing.T) {
	// startpos only
	cmd, err := ParseUciCommand("position startpos")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	p, ok := cmd.(CmdPosition)
	if !ok {
		t.Fatalf("expected CmdPosition, got %T", cmd)
	}
	if !p.StartPos || p.FEN != "" || len(p.Moves) != 0 {
		t.Fatalf("unexpected CmdPosition: %+v", p)
	}

	// startpos with moves
	cmd, err = ParseUciCommand("position startpos moves e2e4 e7e5 g1f3")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	p = cmd.(CmdPosition)
	if !p.StartPos || !reflect.DeepEqual(p.Moves, []string{"e2e4", "e7e5", "g1f3"}) {
		t.Fatalf("unexpected moves: %+v", p)
	}

	// FEN with 6 fields
	fen := "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
	cmd, err = ParseUciCommand("position fen " + fen)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	p = cmd.(CmdPosition)
	if p.StartPos || p.FEN != fen {
		t.Fatalf("unexpected fen: %+v", p)
	}

	// FEN with moves
	cmd, err = ParseUciCommand("position fen " + fen + " moves e2e4 e7e5")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	p = cmd.(CmdPosition)
	if p.FEN != fen || !reflect.DeepEqual(p.Moves, []string{"e2e4", "e7e5"}) {
		t.Fatalf("unexpected fen+moves: %+v", p)
	}

	// errors
	if _, err := ParseUciCommand("position"); err == nil {
		t.Fatalf("expected error for missing args")
	}
	if _, err := ParseUciCommand("position foo"); err == nil {
		t.Fatalf("expected error for invalid keyword")
	}
	if _, err := ParseUciCommand("position fen rnbqkbnr/pppppppp/8/8/8/8"); err == nil {
		t.Fatalf("expected error for too few fen fields")
	}
}

func TestParseUciCommand_Go(t *testing.T) {
	// minimal
	cmd, err := ParseUciCommand("go")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	g, ok := cmd.(CmdGo)
	if !ok {
		t.Fatalf("expected CmdGo, got %T", cmd)
	}
	if g.Ponder || g.Infinite || len(g.SearchMoves) != 0 || anyIntPtrSet(g) {
		t.Fatalf("unexpected defaults: %+v", g)
	}

	// searchmoves breaks at next keyword
	cmd, err = ParseUciCommand("go searchmoves e2e4 d2d4 wtime 1000 btime 2000")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	g = cmd.(CmdGo)
	if !reflect.DeepEqual(g.SearchMoves, []string{"e2e4", "d2d4"}) {
		t.Fatalf("unexpected searchmoves: %+v", g.SearchMoves)
	}
	if !intPtrEqual(g.WTime, 1000) || !intPtrEqual(g.BTime, 2000) {
		t.Fatalf("unexpected times: W:%v B:%v", g.WTime, g.BTime)
	}

	// all flags and many numbers, with unknown token ignored
	cmd, err = ParseUciCommand("go ponder winc 15 binc 25 movestogo 40 depth 12 nodes 9999 mate 4 movetime 500 infinite xyz")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	g = cmd.(CmdGo)
	if !g.Ponder || !g.Infinite {
		t.Fatalf("expected ponder and infinite true")
	}
	if !intPtrEqual(g.WInc, 15) || !intPtrEqual(g.BInc, 25) || !intPtrEqual(g.MovesToGo, 40) || !intPtrEqual(g.Depth, 12) || !intPtrEqual(g.Nodes, 9999) || !intPtrEqual(g.Mate, 4) || !intPtrEqual(g.MoveTime, 500) {
		t.Fatalf("unexpected numeric fields: %+v", g)
	}
}

func TestParseUciCommand_StopPonderQuit(t *testing.T) {
	if cmd, err := ParseUciCommand("stop"); err != nil || !isType[CmdStop](cmd) {
		t.Fatalf("expected CmdStop, got %T err=%v", cmd, err)
	}
	if cmd, err := ParseUciCommand("ponderhit"); err != nil || !isType[CmdPonderHit](cmd) {
		t.Fatalf("expected CmdPonderHit, got %T err=%v", cmd, err)
	}
	if cmd, err := ParseUciCommand("quit"); err != nil || !isType[CmdQuit](cmd) {
		t.Fatalf("expected CmdQuit, got %T err=%v", cmd, err)
	}
}

func TestParseUciCommand_GeneralErrors(t *testing.T) {
	if _, err := ParseUciCommand(""); err == nil {
		t.Fatalf("expected error for empty input")
	}
	if _, err := ParseUciCommand("hello"); err == nil {
		t.Fatalf("expected error for unknown command")
	}
}

// helpers

func strPtr(s string) *string { return &s }

func strPtrEqual(a, b *string) bool {
	if a == nil && b == nil {
		return true
	}
	if a == nil || b == nil {
		return false
	}
	return *a == *b
}

func intPtrEqual(p *int, want int) bool {
	if p == nil {
		return false
	}
	return *p == want
}

func anyIntPtrSet(g CmdGo) bool {
	return g.WTime != nil || g.BTime != nil || g.WInc != nil || g.BInc != nil || g.MovesToGo != nil || g.Depth != nil || g.Nodes != nil || g.Mate != nil || g.MoveTime != nil
}

type onlyType[T any] interface{}

func isType[T any](v any) bool {
	_, ok := v.(T)
	return ok
}
