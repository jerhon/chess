package main

import (
	"context"
	"errors"
	"flag"
	"fmt"
	"io"
	"os"
	"path/filepath"
	"strconv"
	"strings"
	"time"

	chessapi "github.com/jerhon/chess/pkg/chess_dotcomapi"
)

func main() {
	if len(os.Args) < 2 {
		printGlobalHelp()
		os.Exit(2)
	}

	sub := os.Args[1]
	switch sub {
	case "help", "-h", "--help":
		printGlobalHelp()
		return
	case "export-pgns":
		if err := cmdExportPGNs(os.Args[2:]); err != nil {
			fmt.Fprintln(os.Stderr, "error:", err)
			os.Exit(1)
		}
	default:
		fmt.Fprintf(os.Stderr, "unknown command: %s\n\n", sub)
		printGlobalHelp()
		os.Exit(2)
	}
}

func printGlobalHelp() {
	fmt.Fprintf(os.Stderr, `Chess.com CLI

Usage:
  chessdotcom <command> [options]

Commands:
  export-pgns   Export PGNs for a user's games for a specific month

Run 'chessdotcom <command> -h' for command-specific help.
`)
}

func cmdExportPGNs(args []string) error {
	fs := flag.NewFlagSet("export-pgns", flag.ContinueOnError)
	fs.SetOutput(os.Stderr)

	var (
		username     = fs.String("username", "", "Chess.com username (required)")
		yearStr      = fs.String("year", "", "Year, e.g. 2025 (required)")
		monthStr     = fs.String("month", "", "Month 1-12 (required)")
		outputPath   = fs.String("output", "-", "Output file path or '-' for stdout")
		baseURL      = fs.String("base-url", chessapi.DefaultBaseURL, "API base URL (for testing)")
		retries      = fs.Int("retries", 2, "Number of retries for transient errors")
		retryBackoff = fs.Duration("retry-backoff", 250*time.Millisecond, "Initial retry backoff (e.g. 250ms)")
		timeout      = fs.Duration("timeout", 30*time.Second, "Overall request timeout")
	)

	fs.Usage = func() {
		fmt.Fprintf(os.Stderr, `Usage: chessdotcom export-pgns [options]

Options:
  -username <name>        Chess.com username (required)
  -year <YYYY>            Year, e.g. 2025 (required)
  -month <1-12>           Month number (required)
  -output <path|->        Output file path or '-' for stdout (default '-')
  -retries <n>            Number of retries for transient errors (default 2)
  -retry-backoff <dur>    Initial retry backoff, e.g. 250ms (default 250ms)
  -timeout <dur>          Overall request timeout (default 30s)
  -base-url <url>         API base URL (for testing)

Examples:
  chessdotcom export-pgns -username erik -year 2023 -month 7 > erik-2023-07.pgn
  chessdotcom export-pgns -u erik -year 2023 -month 7 -output erik-2023-07.pgn
`)
	}

	// Accept short aliases for convenience by pre-processing args
	args = expandShortFlags(args)

	if err := fs.Parse(args); err != nil {
		if errors.Is(err, flag.ErrHelp) {
			return nil
		}
		return err
	}

	if *username == "" || *yearStr == "" || *monthStr == "" {
		fs.Usage()
		return fmt.Errorf("missing required flags")
	}

	year, err := strconv.Atoi(*yearStr)
	if err != nil || year < 2000 || year > time.Now().Year()+1 {
		return fmt.Errorf("invalid year: %q", *yearStr)
	}
	month, err := strconv.Atoi(*monthStr)
	if err != nil || month < 1 || month > 12 {
		return fmt.Errorf("invalid month: %q", *monthStr)
	}

	// Prepare output writer
	var out io.WriteCloser
	if *outputPath == "-" || *outputPath == "" {
		out = nopWriteCloser{Writer: os.Stdout}
	} else {
		// Ensure directory exists
		dir := filepath.Dir(*outputPath)
		if dir != "." && dir != "" {
			if err := os.MkdirAll(dir, 0o755); err != nil {
				return fmt.Errorf("create output directory: %w", err)
			}
		}
		f, err := os.Create(*outputPath)
		if err != nil {
			return fmt.Errorf("open output file: %w", err)
		}
		out = f
	}
	defer out.Close()

	// Build client
	client := chessapi.NewClient()
	client.BaseURL = strings.TrimRight(*baseURL, "/")
	client.Retries = *retries
	client.RetryBackoff = *retryBackoff

	ctx, cancel := context.WithTimeout(context.Background(), *timeout)
	defer cancel()

	mg, err := client.GetPlayerMonthlyGames(ctx, *username, year, month)
	if err != nil {
		return err
	}

	// Write PGNs separated by blank line
	written := 0
	for _, g := range mg.Games {
		pgn := strings.TrimSpace(g.PGN)
		if pgn == "" {
			continue
		}
		if written > 0 {
			if _, err := io.WriteString(out, "\n\n"); err != nil {
				return err
			}
		}
		if _, err := io.WriteString(out, pgn); err != nil {
			return err
		}
		written++
	}

	if written == 0 {
		// Nothing to write; still succeed but inform via stderr
		fmt.Fprintln(os.Stderr, "no games with PGN found for specified month")
	}
	return nil
}

// expandShortFlags allows aliases: -u for -username, -m for -month
func expandShortFlags(args []string) []string {
	res := make([]string, 0, len(args))
	for i := 0; i < len(args); i++ {
		a := args[i]
		switch a {
		case "-u":
			res = append(res, "-username")
		case "-m":
			res = append(res, "-month")
		case "-y":
			res = append(res, "-year")
		default:
			res = append(res, a)
		}
	}
	return res
}

type nopWriteCloser struct{ io.Writer }

func (nwc nopWriteCloser) Close() error { return nil }
