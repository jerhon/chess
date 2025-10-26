package chess_dotcomapi

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"path"
	"strconv"
	"strings"
	"time"
)

// Package chess_dotcomapi provides a small client for the Chess.com Published Data API.
// API reference: https://www.chess.com/news/view/published-data-api

const DefaultBaseURL = "https://api.chess.com/pub"

// Client provides methods to call the Chess.com Published Data API.
// It is safe for concurrent use by multiple goroutines.
type Client struct {
	// HTTP is the underlying HTTP client. If nil, http.DefaultClient is used.
	HTTP *http.Client
	// BaseURL is the API base URL. If empty, DefaultBaseURL is used.
	BaseURL string
	// UserAgent is sent with each request. If empty, a default is used.
	UserAgent string
	// Retries is the number of retry attempts on transient errors (429/5xx).
	Retries int
	// RetryBackoff is the initial backoff duration between retries.
	RetryBackoff time.Duration
}

// NewClient returns a new Client with sensible defaults.
func NewClient() *Client {
	return &Client{
		HTTP:         http.DefaultClient,
		BaseURL:      DefaultBaseURL,
		UserAgent:    "chess-dotcomapi-go/1",
		Retries:      2,
		RetryBackoff: 250 * time.Millisecond,
	}
}

// HTTPError represents a non-2xx error response from the API.
type HTTPError struct {
	StatusCode int
	Body       string
}

func (e *HTTPError) Error() string {
	return fmt.Sprintf("chess.com API HTTP %d: %s", e.StatusCode, e.Body)
}

// doJSON performs an HTTP request to pathParts (joined) relative to BaseURL
// and decodes the JSON response into out. on 204 No Content, out is untouched.
func (c *Client) doJSON(ctx context.Context, method string, pathParts []string, query url.Values, out any) error {
	base := strings.TrimRight(c.BaseURL, "/")
	p := path.Join(pathParts...)
	u := base + "/" + p
	if len(query) > 0 {
		qs := query.Encode()
		if qs != "" {
			u += "?" + qs
		}
	}

	req, err := http.NewRequestWithContext(ctx, method, u, nil)
	if err != nil {
		return err
	}
	ua := c.UserAgent
	if ua == "" {
		ua = "chess-dotcomapi-go/1"
	}
	req.Header.Set("Accept", "application/json")
	req.Header.Set("User-Agent", ua)

	httpClient := c.HTTP
	if httpClient == nil {
		httpClient = http.DefaultClient
	}

	attempts := c.Retries + 1
	if attempts <= 0 {
		attempts = 1
	}
	backoff := c.RetryBackoff
	if backoff <= 0 {
		backoff = 250 * time.Millisecond
	}

	for attempt := 0; attempt < attempts; attempt++ {
		resp, err := httpClient.Do(req)
		if err != nil {
			// Only retry on transient network errors.
			if attempt < attempts-1 {
				time.Sleep(backoff)
				backoff *= 2
				continue
			}
			return err
		}
		defer resp.Body.Close()

		if resp.StatusCode == http.StatusNoContent {
			return nil
		}
		if resp.StatusCode >= 200 && resp.StatusCode < 300 {
			if out == nil {
				io.Copy(io.Discard, resp.Body)
				return nil
			}
			dec := json.NewDecoder(resp.Body)
			// Be lenient, API may add fields. We only decode known ones.
			return dec.Decode(out)
		}

		// Non-2xx: maybe retry
		b, _ := io.ReadAll(io.LimitReader(resp.Body, 2<<10)) // 2KB snippet
		if shouldRetry(resp.StatusCode) && attempt < attempts-1 {
			if ra := retryAfterDelay(resp); ra > 0 {
				time.Sleep(ra)
			} else {
				time.Sleep(backoff)
				backoff *= 2
			}
			continue
		}
		return &HTTPError{StatusCode: resp.StatusCode, Body: string(b)}
	}
	return errors.New("unexpected: retries exhausted without returning")
}

func shouldRetry(code int) bool {
	if code == http.StatusTooManyRequests {
		return true
	}
	return code >= 500 && code <= 599
}

func retryAfterDelay(resp *http.Response) time.Duration {
	ra := resp.Header.Get("Retry-After")
	if ra == "" {
		return 0
	}
	if secs, err := strconv.Atoi(ra); err == nil && secs >= 0 {
		return time.Duration(secs) * time.Second
	}
	if t, err := http.ParseTime(ra); err == nil {
		return time.Until(t)
	}
	return 0
}

// Player corresponds to https://api.chess.com/pub/player/{username}
// Only a subset of fields is modeled; unknown fields are ignored.
type Player struct {
	Username   string `json:"username"`
	PlayerID   int    `json:"player_id"`
	URL        string `json:"url"`
	Name       string `json:"name,omitempty"`
	Title      string `json:"title,omitempty"`
	Followers  int    `json:"followers,omitempty"`
	Country    string `json:"country,omitempty"`
	Location   string `json:"location,omitempty"`
	LastOnline int64  `json:"last_online,omitempty"`
	Joined     int64  `json:"joined,omitempty"`
	Status     string `json:"status,omitempty"`
	IsStreamer bool   `json:"is_streamer,omitempty"`
	Avatar     string `json:"avatar,omitempty"`
	TwitchURL  string `json:"twitch_url,omitempty"`
	Fide       int    `json:"fide,omitempty"`
}

// GetPlayer fetches a player profile.
func (c *Client) GetPlayer(ctx context.Context, username string) (Player, error) {
	var out Player
	if username == "" {
		return out, fmt.Errorf("username is required")
	}
	uname := strings.ToLower(username)
	err := c.doJSON(ctx, http.MethodGet, []string{"player", uname}, nil, &out)
	return out, err
}

// PlayerStats corresponds to https://api.chess.com/pub/player/{username}/stats
// The structure groups time classes with ratings and records.
type PlayerStats struct {
	ChessRapid  *StatsCategory `json:"chess_rapid,omitempty"`
	ChessBlitz  *StatsCategory `json:"chess_blitz,omitempty"`
	ChessBullet *StatsCategory `json:"chess_bullet,omitempty"`
	Daily       *StatsCategory `json:"chess_daily,omitempty"`
}

type StatsCategory struct {
	Last   *RatingSample `json:"last,omitempty"`
	Best   *BestSample   `json:"best,omitempty"`
	Record *Record       `json:"record,omitempty"`
}

type RatingSample struct {
	Rating int   `json:"rating"`
	RD     int   `json:"rd,omitempty"`
	Date   int64 `json:"date,omitempty"`
}

type BestSample struct {
	Rating int    `json:"rating"`
	Date   int64  `json:"date,omitempty"`
	Game   string `json:"game,omitempty"`
}

type Record struct {
	Win  int `json:"win,omitempty"`
	Loss int `json:"loss,omitempty"`
	Draw int `json:"draw,omitempty"`
}

// GetPlayerStats fetches rating stats for a player.
func (c *Client) GetPlayerStats(ctx context.Context, username string) (PlayerStats, error) {
	var out PlayerStats
	if username == "" {
		return out, fmt.Errorf("username is required")
	}
	uname := strings.ToLower(username)
	err := c.doJSON(ctx, http.MethodGet, []string{"player", uname, "stats"}, nil, &out)
	return out, err
}

// ArchiveList represents the list of monthly archive URLs.
type ArchiveList struct {
	Archives []string `json:"archives"`
}

// ListPlayerGameArchives returns the monthly archive URLs for a player.
func (c *Client) ListPlayerGameArchives(ctx context.Context, username string) ([]string, error) {
	if username == "" {
		return nil, fmt.Errorf("username is required")
	}
	uname := strings.ToLower(username)
	var out ArchiveList
	if err := c.doJSON(ctx, http.MethodGet, []string{"player", uname, "games", "archives"}, nil, &out); err != nil {
		return nil, err
	}
	return out.Archives, nil
}

// MonthlyGames corresponds to https://api.chess.com/pub/player/{username}/games/{YYYY}/{MM}
// We model a subset of fields required for common tasks.
type MonthlyGames struct {
	Games []Game `json:"games"`
}

type Game struct {
	URL         string `json:"url,omitempty"`
	PGN         string `json:"pgn,omitempty"`
	TimeControl string `json:"time_control,omitempty"`
	EndTime     int64  `json:"end_time,omitempty"`
	Rated       bool   `json:"rated,omitempty"`
	TCN         string `json:"tcn,omitempty"`
	UUID        string `json:"uuid,omitempty"`
	TimeClass   string `json:"time_class,omitempty"`
	Rules       string `json:"rules,omitempty"`
	White       Side   `json:"white"`
	Black       Side   `json:"black"`
}

type Side struct {
	Username string `json:"username"`
	Result   string `json:"result,omitempty"`
	Rating   int    `json:"rating,omitempty"`
	URL      string `json:"url,omitempty"`
}

// GetPlayerMonthlyGames fetches games for a given year and month (1-12).
func (c *Client) GetPlayerMonthlyGames(ctx context.Context, username string, year int, month int) (MonthlyGames, error) {
	var out MonthlyGames
	if username == "" {
		return out, fmt.Errorf("username is required")
	}
	if month < 1 || month > 12 {
		return out, fmt.Errorf("month must be 1-12")
	}
	uname := strings.ToLower(username)
	mm := fmt.Sprintf("%02d", month)
	p := []string{"player", uname, "games", fmt.Sprintf("%04d", year), mm}
	err := c.doJSON(ctx, http.MethodGet, p, nil, &out)
	return out, err
}

// Leaderboards corresponds to https://api.chess.com/pub/leaderboards
// Only a subset of categories and fields are modeled here.
type Leaderboards struct {
	LiveRapid  []LeaderboardPlayer `json:"live_rapid,omitempty"`
	LiveBlitz  []LeaderboardPlayer `json:"live_blitz,omitempty"`
	LiveBullet []LeaderboardPlayer `json:"live_bullet,omitempty"`
	Daily      []LeaderboardPlayer `json:"daily,omitempty"`
}

type LeaderboardPlayer struct {
	PlayerID int    `json:"player_id,omitempty"`
	Username string `json:"username,omitempty"`
	URL      string `json:"url,omitempty"`
	Score    int    `json:"score,omitempty"`
}

// GetLeaderboards fetches current leaderboards.
func (c *Client) GetLeaderboards(ctx context.Context) (Leaderboards, error) {
	var out Leaderboards
	err := c.doJSON(ctx, http.MethodGet, []string{"leaderboards"}, nil, &out)
	return out, err
}
