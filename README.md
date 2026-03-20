# Chess

A command-line chess game and engine written in Go. It includes a full chess rule engine, notation support, a terminal UI, and a Chess.com data exporter.

## Features

- **Full chess rules** — legal move generation, check, checkmate, stalemate, en passant, castling, pawn promotion, threefold repetition draw
- **SAN** (Standard Algebraic Notation) — parsing and generation
- **FEN** (Forsyth–Edwards Notation) — parsing and serialization
- **PGN** (Portable Game Notation) — tokenizer and parser
- **Interactive TUI** — terminal chess board built with [Bubble Tea](https://github.com/charmbracelet/bubbletea) and [Lip Gloss](https://github.com/charmbracelet/lipgloss)
- **Chess.com exporter** — fetch and export PGNs from the [Chess.com Published Data API](https://www.chess.com/news/view/published-data-api)
- **UCI stub** — skeleton Universal Chess Interface implementation

## Repository Layout

```
cmd/
  chess-cli/         # Interactive TUI chess game
  chessdotcom/       # CLI tool for exporting PGNs from Chess.com
pkg/
  chess/             # Public chess game API (ChessGame, TrySanMove, GetMoves, …)
    game/            # Core primitives: board, pieces, locations, move generation
    san/             # SAN parser and data types
    pgn/             # PGN tokenizer and parser
    fen/             # FEN parser and serializer
  chess_dotcomapi/   # HTTP client for the Chess.com Published Data API
  chess_uci/         # UCI protocol stub
```

## Requirements

- Go 1.24 or later

## Building

```bash
# Build everything
go build ./...

# Build individual binaries
go build ./cmd/chess-cli
go build ./cmd/chessdotcom
```

## Usage

### chess-cli — interactive TUI chess game

```bash
go run ./cmd/chess-cli
```

The board is rendered in the terminal with alternating light/dark squares. The sidebar shows all valid moves for the player to move.

Enter moves in **SAN notation** at the prompt, for example:

| Input | Meaning |
|-------|---------|
| `e4`  | Pawn to e4 |
| `Nf3` | Knight to f3 |
| `O-O` | Castle kingside |
| `O-O-O` | Castle queenside |
| `e8=Q` | Pawn promotes to queen on e8 |

Press **Ctrl-C**, **Esc**, or type `quit` / `exit` / `q` to exit.

### chessdotcom — Chess.com PGN exporter

```bash
go run ./cmd/chessdotcom export-pgns \
  -username erik \
  -year 2023 \
  -month 7 \
  > erik-2023-07.pgn
```

#### Options

| Flag | Default | Description |
|------|---------|-------------|
| `-username` / `-u` | *(required)* | Chess.com username |
| `-year` / `-y` | *(required)* | Year (e.g. `2025`) |
| `-month` / `-m` | *(required)* | Month 1–12 |
| `-output` | `-` (stdout) | Output file path, or `-` for stdout |
| `-retries` | `2` | Retries on transient errors (429 / 5xx) |
| `-retry-backoff` | `250ms` | Initial retry backoff |
| `-timeout` | `30s` | Overall request timeout |
| `-base-url` | `https://api.chess.com/pub` | API base URL |

Run `chessdotcom help` or `chessdotcom export-pgns -h` for full usage information.

## Using the Library

The public API lives in `pkg/chess`.

```go
import chess "github.com/jerhon/chess/pkg/chess"

// Start a new game from the standard position.
g := chess.NewGame()

// Apply moves in SAN notation.
if _, err := g.TrySanMove("e4"); err != nil {
    log.Fatal(err)
}
if _, err := g.TrySanMove("e5"); err != nil {
    log.Fatal(err)
}

// Query the position.
pos := g.GetPosition()
fmt.Println("Player to move:", pos.PlayerToMove)

// Query game state.
fmt.Println("In check?", g.IsCheck())
fmt.Println("Checkmate?", g.IsCheckmate())
fmt.Println("Stalemate?", g.IsStalemate())
fmt.Println("Result:", g.GetResult())

// List legal moves.
for _, move := range g.GetLegalMoves() {
    fmt.Printf("%v -> %v\n", move.From.Location, move.To)
}
```

## Testing

```bash
# Run all tests
go test ./...

# Run with the race detector (matches CI)
go test -v -race ./...
```

## Linting

```bash
golangci-lint run
```

## CI

GitHub Actions runs three jobs on every push and pull request to `main`:

1. **Build** — `go build ./...`
2. **Test** — `go test -v -race ./...`
3. **Lint** — `golangci-lint run`

All three must pass before a PR can be merged.
