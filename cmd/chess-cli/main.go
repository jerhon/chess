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
	// ── Left panel: board ──────────────────────────────────────────────────
	boardContent := titleStyle.Render("Chess") + "\n\n" +
		renderBoard(m.chessGame.GetPosition()) + "\n\n" +
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
		inputWidth = m.windowWidth - 2 // account for borders
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

// renderBoard produces a spaced board with rank/file labels.
func renderBoard(pos *game.ChessPosition) string {
	sb := strings.Builder{}
	for rank := game.Rank8; rank >= game.Rank1; rank-- {
		sb.WriteRune(rune(rank))
		sb.WriteString("  ")
		for file := game.FileA; file <= game.FileH; file++ {
			square := pos.Board.GetSquare(game.ChessLocation{File: file, Rank: rank})
			if square.Piece.Piece == game.NoPiece {
				sb.WriteString("· ")
			} else {
				sb.WriteString(square.Piece.PrettyString())
				sb.WriteRune(' ')
			}
		}
		sb.WriteRune('\n')
	}
	sb.WriteString("   a b c d e f g h")
	return sb.String()
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
		return labelStyle.Render(fmt.Sprintf("%s to move   Move %d", player, pos.FullmoveNumber+1))
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

	// Only show moves that can actually be played: on-board and CanMove is true.
	var valid []game.ChessMove
	for _, mv := range moves {
		if mv.CanMove && mv.To.IsOnBoard() {
			valid = append(valid, mv)
		}
	}

	if len(valid) == 0 {
		return labelStyle.Render("(none)")
	}

	var sb strings.Builder
	for i, mv := range valid {
		entry := fmt.Sprintf("%-8s", mv.String())
		sb.WriteString(moveStyle.Render(entry))
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
