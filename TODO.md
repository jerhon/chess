# Chess Engine TODO

This file tracks missing logic, known bugs, and future improvements in the chess game engine.

## Critical Bugs

### Castling Not Applied in `ChessGame`
**File:** `pkg/chess/chess.go` — `castleKingSide()` / `castleQueenSide()`

`position.CastleKingside()` and `position.CastleQueenside()` return a new `*ChessPosition`, but
the results are discarded — the game position is never updated. Castling effectively does nothing.

```go
// Bug: result is ignored
g.position.CastleKingside()

// Fix: assign the result
g.position = g.position.CastleKingside()
```

### Castling Rights Lost After Every Move
**File:** `pkg/chess/game/position.go` — `Move()`

The `castlingRights` map is initialized empty and only partially updated in a few edge cases.
For the vast majority of moves the returned `ChessPosition` has an empty `CastlingRights` map,
meaning both players silently lose all castling rights after any ordinary move.

### En Passant Capture Removes the Wrong Square
**File:** `pkg/chess/game/position.go` — `Move()`

The pawn removed during an en passant capture is computed from `fromLocation.File` and a
hard-coded rank (`Rank7` / `Rank3`). Both values are wrong:

- The captured pawn is always on the **same file as the destination square** (`toLocation.File`).
- The captured pawn is always on the **same rank as the moving pawn** (`fromLocation.Rank`).

```go
// Bug: removes the wrong square
if position.PlayerToMove == BlackPiece {
    newBoard.SetSquare(ChessLocation{fromLocation.File, Rank7}, ChessPiece{NoPiece, NoColor})
} else {
    newBoard.SetSquare(ChessLocation{fromLocation.File, Rank3}, ChessPiece{NoPiece, NoColor})
}

// Fix: remove the captured pawn at the correct square
newBoard.SetSquare(ChessLocation{toLocation.File, fromLocation.Rank}, ChessPiece{NoPiece, NoColor})
```

---

## Missing Features

### Pawn Promotion Not Executed
**File:** `pkg/chess/game/position.go` — `Move()` / `pkg/chess/chess.go` — `TrySanMove()`

SAN parsing recognises the promotion piece (e.g. `e8=Q`), but `Move()` never replaces the pawn
with the chosen piece when it reaches the back rank. Promotion moves also need:

- A `PromotionPiece` field on `ChessMove` so the engine can distinguish promotions from ordinary moves.
- Promotion move generation in `calculatePawnMoves()` (add four candidate moves, one per promotable piece).
- Actual replacement of the pawn in `Move()` (and `TrySanMove()` must pass the chosen piece through).

### 50-Move Rule Not Enforced
**File:** `pkg/chess/game/movement.go` — `calculateValidMoves()`

`HalfmoveClock` is tracked correctly in `ChessPosition`, but the rule is never checked. The clock
counts halfmoves (plies); when it reaches 100 (i.e. 50 full moves without a capture or pawn
advance) the game should be declared a draw.

### Threefold Repetition Not Detected
**File:** `pkg/chess/chess.go`

There is no position history. To detect threefold repetition the engine needs to record a hash
(or FEN string) of each position after every move and declare a draw when the same position
has occurred three times.

### Insufficient Material Not Detected
**File:** `pkg/chess/game/movement.go`

The engine does not recognise drawn endgames caused by insufficient mating material, such as:

- King vs. King
- King + Bishop vs. King
- King + Knight vs. King
- King + Bishop vs. King + Bishop (same-coloured bishops)

### No Game Result / Status Type
**File:** `pkg/chess/chess.go` / `pkg/chess/game/movement.go`

There is no enumerated result type (e.g. `WhiteWins`, `BlackWins`, `Draw`, `InProgress`).
Callers currently have to inspect `IsCheckmate` / `IsStalemate` individually and have no way
to represent or propagate draw types (50-move, repetition, insufficient material, agreement).

---

## Minor Issues

### PGN Parser Silently Ignores Invalid Escape Sequences
**File:** `pkg/chess/pgn/tokens.go:132`

An unrecognised backslash escape inside a quoted PGN string is silently dropped. A parser error
or at least a warning should be surfaced.

```go
// TODO add a parser error  ← existing comment, not yet addressed
```

### Castling Rights Update Is Incomplete for Rook Moves
**File:** `pkg/chess/game/position.go` — `Move()`

The rook-move castling-rights update only handles a few cases (and checks `FileE` for a rook,
which is not a starting rook file). It should handle both White and Black rooks moving away from
their starting squares (`FileA` / `FileH`).

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
- The valid-move list is not exposed publicly, preventing external GUIs or engines from
  enumerating legal moves without parsing SAN.
