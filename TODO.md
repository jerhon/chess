# Chess Engine TODO

This file tracks missing logic, known bugs, and future improvements in the chess game engine.

## Minor Issues

### PGN Parser Silently Ignores Invalid Escape Sequences
**File:** `pkg/chess/pgn/tokens.go:132`

An unrecognised backslash escape inside a quoted PGN string is silently dropped. A parser error
or at least a warning should be surfaced.

```go
// TODO add a parser error  ← existing comment, not yet addressed
```

---

## Performance

### Move Validation Is O(n²)
**File:** `pkg/chess/game/movement.go:144`

```go
// TODO: this is pretty expensive, look at optimizing in the future  ← existing comment
```

`calculateValidMoves()` simulates every candidate move and recalculates all opponent moves to
detect check, resulting in quadratic complexity. Incremental check detection or a pin/attack
bitboard would significantly reduce this cost.

---

## Public API Gaps

- `castleKingSide()` / `castleQueenSide()` on `ChessGame` are unexported; callers cannot
  initiate castling independently of `TrySanMove()`.
- There is no coordinate-based move method; callers must always use SAN notation.
