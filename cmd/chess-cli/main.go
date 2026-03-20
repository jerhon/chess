package main

import (
	"fmt"
	"os"
	"strings"

	tea "github.com/charmbracelet/bubbletea"
	"github.com/charmbracelet/lipgloss"
	chess2 "github.com/jerhon/chess/pkg/chess"
	"github.com/jerhon/chess/pkg/chess/game"
)

// ── Styles ────────────────────────────────────────────────────────────────────

var (
	borderColor = lipgloss.Color("63") // purple-ish

	boardStyle = lipgloss.NewStyle().
			Border(lipgloss.RoundedBorder()).
			BorderForeground(borderColor).
			Padding(0, 1)

	sidebarStyle = lipgloss.NewStyle().
			Border(lipgloss.RoundedBorder()).
			BorderForeground(borderColor).
			Padding(0, 1).
			Width(28)

	inputStyle = lipgloss.NewStyle().
			Border(lipgloss.RoundedBorder()).
			BorderForeground(borderColor).
			Padding(0, 1)

	titleStyle = lipgloss.NewStyle().
			Bold(true).
			Foreground(lipgloss.Color("63"))

	labelStyle = lipgloss.NewStyle().
			Bold(true).
			Foreground(lipgloss.Color("243"))

	errorStyle = lipgloss.NewStyle().
			Foreground(lipgloss.Color("196"))

	successStyle = lipgloss.NewStyle().
			Foreground(lipgloss.Color("82"))

	checkStyle = lipgloss.NewStyle().
			Bold(true).
			Foreground(lipgloss.Color("214"))

	moveStyle = lipgloss.NewStyle().
			Foreground(lipgloss.Color("252"))

	cursorStyle = lipgloss.NewStyle().
			Foreground(lipgloss.Color("63"))

	// Chess board square colors — classic Lichess palette.
	lightSquareStyle = lipgloss.NewStyle().
				Background(lipgloss.Color("#F0D9B5"))

	darkSquareStyle = lipgloss.NewStyle().
			Background(lipgloss.Color("#B58863"))

	// Piece foreground colors: white pieces render bright, black pieces render dark.
	whitePieceFg = lipgloss.Color("#FFFFFF")
	blackPieceFg = lipgloss.Color("#1a1a1a")
)

// ── Model ─────────────────────────────────────────────────────────────────────

type model struct {
	chessGame   *chess2.ChessGame
	input       string
	status      string
	isError     bool
	windowWidth int
}

func initialModel() model {
	return model{
		chessGame: chess2.NewGame(),
		status:    "Enter a SAN move (e.g. e4, Nf3, O-O) or type 'quit' to exit.",
	}
}

// ── Init ──────────────────────────────────────────────────────────────────────

func (m model) Init() tea.Cmd {
	return nil
}

// ── Update ────────────────────────────────────────────────────────────────────

func (m model) Update(msg tea.Msg) (tea.Model, tea.Cmd) {
	switch msg := msg.(type) {
	case tea.WindowSizeMsg:
		m.windowWidth = msg.Width
		return m, nil

	case tea.KeyMsg:
		switch msg.Type {
		case tea.KeyCtrlC, tea.KeyEsc:
			return m, tea.Quit

		case tea.KeyEnter:
			input := strings.TrimSpace(m.input)
			m.input = ""
			if input == "quit" || input == "exit" || input == "q" {
				return m, tea.Quit
			}
			if input == "" {
				return m, nil
			}
			_, err := m.chessGame.TrySanMove(input)
			if err != nil {
				m.status = err.Error()
				m.isError = true
			} else {
				m.status = fmt.Sprintf("Played: %s", input)
				m.isError = false
			}

		case tea.KeyBackspace:
			if len(m.input) > 0 {
				m.input = m.input[:len(m.input)-1]
			}

		case tea.KeyRunes:
			m.input += string(msg.Runes)
		}
	}
	return m, nil
}

// ── View ──────────────────────────────────────────────────────────────────────

func (m model) View() string {
	pos := m.chessGame.GetPosition()

	// ── Left panel: board ──────────────────────────────────────────────────
	boardContent := titleStyle.Render("Chess") + "\n\n" +
		renderBoard(pos, pos.PlayerToMove) + "\n\n" +
		renderGameStatus(m.chessGame)
	boardPanel := boardStyle.Render(boardContent)

	// ── Right panel: moves + evaluation ───────────────────────────────────
	sideContent := titleStyle.Render("Valid Moves") + "\n" +
		renderMoves(m.chessGame) + "\n\n" +
		titleStyle.Render("Evaluation") + "\n" +
		labelStyle.Render("(not yet implemented)")
	sidePanel := sidebarStyle.Render(sideContent)

	// ── Top row: board + sidebar ──────────────────────────────────────────
	topRow := lipgloss.JoinHorizontal(lipgloss.Top, boardPanel, sidePanel)

	// ── Bottom: input + status ────────────────────────────────────────────
	cursor := cursorStyle.Render("│")
	prompt := fmt.Sprintf("> %s%s", m.input, cursor)

	// Make the input panel span the full width of the top row when possible.
	inputWidth := 60
	if m.windowWidth > 0 {
		inputWidth = m.windowWidth - 4 // account for borders
	}
	prompt = lipgloss.NewStyle().Width(inputWidth).Render(prompt)
	inputPanel := inputStyle.Render(prompt)

	var statusLine string
	if m.isError {
		statusLine = errorStyle.Render("✗ " + m.status)
	} else {
		statusLine = successStyle.Render("✓ " + m.status)
	}

	bottom := lipgloss.JoinVertical(lipgloss.Left, inputPanel, statusLine)

	return lipgloss.JoinVertical(lipgloss.Left, topRow, bottom) + "\n"
}

// ── Helpers ───────────────────────────────────────────────────────────────────

// renderBoard produces a board where each square is 3 columns wide × 3 rows
// tall with rank/file labels and alternating light/dark square background
// colors. The piece letter is centered in the middle row and column of each
// square. The board is oriented so that the player whose turn it is appears
// at the bottom.
func renderBoard(pos *game.ChessPosition, perspective game.ColorType) string {
	// Determine iteration order based on perspective.
	// White at bottom: ranks 8→1, files a→h.
	// Black at bottom: ranks 1→8, files h→a.
	rankStart, rankEnd, rankStep := game.Rank8, game.Rank1, game.RankType(-1)
	fileStart, fileEnd, fileStep := game.FileA, game.FileH, game.FileType(1)
	if perspective == game.BlackPiece {
		rankStart, rankEnd, rankStep = game.Rank1, game.Rank8, game.RankType(1)
		fileStart, fileEnd, fileStep = game.FileH, game.FileA, game.FileType(-1)
	}

	// indent is placed before the top/bottom rows where the rank label would be.
	const indent = "   "

	var rows []string
	for rank := rankStart; ; rank += rankStep {
		// Each rank produces 3 output lines: top padding, middle (with piece),
		// and bottom padding.
		var top, mid, bot strings.Builder

		// Top and bottom rows have the same indent as the rank-label column.
		top.WriteString(indent)
		bot.WriteString(indent)

		// Middle row carries the rank label.
		mid.WriteRune(rune(rank))
		mid.WriteString("  ")

		for file := fileStart; ; file += fileStep {
			square := pos.Board.GetSquare(game.ChessLocation{File: file, Rank: rank})
			sq := cellStyle(file, rank)

			// Top and bottom rows: 3 background-coloured spaces.
			top.WriteString(sq.Render("   "))
			bot.WriteString(sq.Render("   "))

			// Middle row: piece letter (bold, player-coloured) centred in " X ".
			if square.Piece.Piece == game.NoPiece {
				mid.WriteString(sq.Render("   "))
			} else {
				fg := whitePieceFg
				if square.Piece.Color == game.BlackPiece {
					fg = blackPieceFg
				}
				mid.WriteString(sq.Foreground(fg).Bold(true).Render(" " + square.Piece.PrettyString() + " "))
			}

			if file == fileEnd {
				break
			}
		}

		rows = append(rows, top.String(), mid.String(), bot.String())

		if rank == rankEnd {
			break
		}
	}

	// File labels centred under each 3-column square.
	var fileLine strings.Builder
	fileLine.WriteString(indent)
	for file := fileStart; ; file += fileStep {
		fileLine.WriteRune(' ')
		fileLine.WriteRune(rune(file))
		fileLine.WriteRune(' ')
		if file == fileEnd {
			break
		}
	}
	rows = append(rows, fileLine.String())

	return strings.Join(rows, "\n")
}

// cellStyle returns the lipgloss style for a square based on its coordinates.
// a1 is a dark square: (fileIndex + rankIndex) even → dark, odd → light.
func cellStyle(file game.FileType, rank game.RankType) lipgloss.Style {
	fileIdx := int(file - game.FileA)
	rankIdx := int(rank - game.Rank1)
	if (fileIdx+rankIdx)%2 == 0 {
		return darkSquareStyle
	}
	return lightSquareStyle
}

// renderGameStatus shows whose turn it is and any check/mate/stalemate state.
func renderGameStatus(g *chess2.ChessGame) string {
	pos := g.GetPosition()

	var player string
	if pos.PlayerToMove == game.WhitePiece {
		player = "White"
	} else {
		player = "Black"
	}

	switch {
	case g.IsCheckmate():
		return checkStyle.Render(fmt.Sprintf("Checkmate! %s wins.", oppositePlayer(pos.PlayerToMove)))
	case g.IsStalemate():
		return checkStyle.Render("Stalemate! Draw.")
	case g.IsCheck():
		return checkStyle.Render(fmt.Sprintf("%s to move — CHECK!", player))
	default:
		return labelStyle.Render(fmt.Sprintf("%s to move   Move %d", player, pos.FullmoveNumber))
	}
}

// oppositePlayer returns the name of the opposite color.
func oppositePlayer(color game.ColorType) string {
	if color == game.WhitePiece {
		return "Black"
	}
	return "White"
}

// renderMoves lists all valid moves for the current player in columns.
func renderMoves(g *chess2.ChessGame) string {
	moves := g.GetMoves()

	// Only show moves that can actually be played.
	var valid []game.ChessMove
	for _, mv := range moves {
		if mv.CanMove {
			valid = append(valid, mv)
		}
	}

	if len(valid) == 0 {
		return labelStyle.Render("(none)")
	}

	var sb strings.Builder
	for i, mv := range valid {

		hasSameTo := false
		for j, mv2 := range valid {
			if i != j && mv.To == mv2.To {
				hasSameTo = true
				break
			}
		}

		var entry string
		if hasSameTo {
			entry = mv.From.Location.File.String() + mv.From.Location.Rank.String() + mv.To.File.String() + mv.To.Rank.String()
		} else {
			entry = mv.To.File.String() + mv.To.Rank.String()
		}

		sb.WriteString(moveStyle.Render(entry + " "))
		if (i+1)%3 == 0 {
			sb.WriteRune('\n')
		}
	}
	return sb.String()
}

// ── Main ──────────────────────────────────────────────────────────────────────

func main() {
	p := tea.NewProgram(initialModel(), tea.WithAltScreen())
	if _, err := p.Run(); err != nil {
		fmt.Fprintf(os.Stderr, "error running program: %v\n", err)
		os.Exit(1)
	}
}
