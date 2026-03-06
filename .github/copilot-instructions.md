# GitHub Copilot Instructions

## Project Overview

This repository is a command-line chess game and engine written in Go. It includes:

- A chess game library with full rule support (moves, check, checkmate, stalemate, en passant, castling)
- SAN (Standard Algebraic Notation) parsing and generation
- FEN (Forsyth–Edwards Notation) parsing and serialization
- PGN (Portable Game Notation) parsing
- A Chess.com API client for fetching game archives
- A UCI (Universal Chess Interface) stub
- Two CLI applications: an interactive chess game (`chess-cli`) and a Chess.com data exporter (`chessdotcom`)

## Repository Layout

```
cmd/
  chess-cli/      # Interactive CLI chess game (reads SAN moves from stdin)
  chessdotcom/    # CLI tool for exporting PGNs from Chess.com
pkg/
  chess/          # Top-level chess game API (ChessGame, TrySanMove, GetLegalMoves)
    game/         # Core chess primitives: board, pieces, locations, positions, move generation
    san/          # SAN parser and data types
    pgn/          # PGN tokenizer and parser
    fen/          # FEN parser and serializer
  chess_dotcomapi/ # HTTP client for the Chess.com public API
  chess_uci/       # UCI protocol stub
```

## Go Module

Module path: `github.com/jerhon/chess`  
Go version: `1.24`

## Key Types and Conventions

### `pkg/chess/game`

- **`ChessPosition`** – Immutable-by-convention position struct. All mutating operations (`Move`, `CastleKingside`, `CastleQueenside`) return a *new* `*ChessPosition`; they never modify the receiver.
- **`ChessBoard`** – 8×8 board. Squares are addressed with `ChessLocation{File, Rank}`.
- **`ChessMove`** – Describes a single candidate move: `From` (`ChessSquare`), `To` (`ChessLocation`), and boolean flags `CanMove`, `CanCapture`, `IsCastle`.
- **`ChessMovement`** – Calculates candidate and valid moves for a position. Call `Calculate()` once before reading `Moves`, `IsCheckmate`, `IsStalemate`, or `CanCastle`.
- **`FileType` / `RankType`** – Typed integer constants (`FileA`–`FileH`, `Rank1`–`Rank8`). Use the named constants; avoid raw integers.
- **`ColorType`** – `WhitePiece` or `BlackPiece`. Use `color.OppositeColor()` to flip.
- **`PieceType`** – `Pawn`, `Rook`, `Knight`, `Bishop`, `Queen`, `King`, `NoPiece`.
- **`CastlingRights`** – Per-color struct with `KingSide` and `QueenSide` booleans.

### `pkg/chess` (public API)

- **`ChessGame`** – High-level game controller. Create with `NewGame()`.
- **`TrySanMove(san string) (bool, error)`** – Apply a move given in SAN notation.
- **`GetLegalMoves() []game.ChessMove`** – Returns all legal moves for the current player.
- **`GetPosition() *game.ChessPosition`** – Returns the current position.

## Coding Conventions

- Follow standard Go idioms and formatting (`gofmt`/`goimports`).
- Use table-driven tests with `github.com/stretchr/testify/assert` and `require`.
- Keep packages focused: chess logic in `pkg/chess/game`, notation parsing in their respective sub-packages.
- Position modifications must always return a new value; never mutate a `*ChessPosition` in place.
- Use named constants for files, ranks, pieces, and colors rather than raw literals.
- Exported functions and types should have doc comments.

## Testing

Run all tests:

```bash
go test ./...
```

Run tests with the race detector (matches CI):

```bash
go test -v -race ./...
```

Tests use `github.com/stretchr/testify`. Prefer `assert` for non-fatal checks and `require` for fatal ones.

## Building

```bash
go build ./...
```

Build a specific binary:

```bash
go build ./cmd/chess-cli
go build ./cmd/chessdotcom
```

## Linting

The project uses `golangci-lint` with the standard linter set defined in `.golangci.yml`.

```bash
golangci-lint run
```

## CI

GitHub Actions (`.github/workflows/verify.yml`) runs three jobs on every push and pull request to `main`:

1. **Build** – `go build ./...`
2. **Test** – `go test -v -race ./...`
3. **Lint** – `golangci-lint run`

All three jobs must pass before a PR can be merged.

## Known Issues and Limitations

See `TODO.md` for the full list. Key items:

- Castling result is not applied in `ChessGame.castleKingSide` / `castleQueenSide` (result of `CastleKingside()` is discarded).
- Castling rights are not preserved across ordinary moves in `position.Move()`.
- En passant capture removes the wrong square.
- Pawn promotion is parsed but not executed.
- The 50-move rule, threefold repetition, and insufficient-material draws are not enforced.
- Move validation is O(n²); see the TODO for optimization notes.
