package chess_dotcomapi

import (
	"context"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"sync/atomic"
	"testing"
	"time"
)

// helper to build a client pointed at the provided server
func testClientFor(ts *httptest.Server) *Client {
	c := NewClient()
	c.BaseURL = ts.URL // client builds URL as base + "/" + path
	c.HTTP = ts.Client()
	return c
}

func TestGetPlayer_SuccessAndLowercasing(t *testing.T) {
	var gotPath string
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		gotPath = r.URL.Path
		if r.Header.Get("Accept") != "application/json" {
			t.Errorf("missing/invalid Accept header: %q", r.Header.Get("Accept"))
		}
		// Respond with extra unknown field to ensure lenient decoding
		w.Header().Set("Content-Type", "application/json")
		_, _ = w.Write([]byte(`{"username":"erik","player_id":1,"url":"u","unknown":"x"}`))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	// Force default UA path (empty triggers default inside doJSON)
	c.UserAgent = ""
	p, err := c.GetPlayer(context.Background(), "Erik")
	if err != nil {
		t.Fatalf("GetPlayer error: %v", err)
	}
	if p.Username != "erik" {
		t.Fatalf("unexpected username: %+v", p)
	}
	if gotPath != "/player/erik" {
		t.Fatalf("expected lowercase path '/player/erik', got %q", gotPath)
	}
}

func TestGetPlayer_EmptyUsername(t *testing.T) {
	c := NewClient()
	_, err := c.GetPlayer(context.Background(), "")
	if err == nil {
		t.Fatalf("expected error for empty username")
	}
}

func TestHeaders_UserAgentAndAccept(t *testing.T) {
	var ua string
	var accept string
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		ua = r.Header.Get("User-Agent")
		accept = r.Header.Get("Accept")
		w.Header().Set("Content-Type", "application/json")
		_, _ = w.Write([]byte(`{"username":"x"}`))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	// Clear UA to exercise default UA value in doJSON
	c.UserAgent = ""

	if _, err := c.GetPlayer(context.Background(), "x"); err != nil {
		t.Fatalf("GetPlayer error: %v", err)
	}
	if ua == "" || ua == "Go-http-client/1.1" {
		t.Fatalf("expected custom default UA, got %q", ua)
	}
	if accept != "application/json" {
		t.Fatalf("expected Accept application/json, got %q", accept)
	}
}

func TestRetries_5xxThenSuccess(t *testing.T) {
	var attempts int32
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		n := atomic.AddInt32(&attempts, 1)
		if n < 3 {
			// first two attempts fail with 500
			http.Error(w, "temporary", http.StatusInternalServerError)
			return
		}
		w.Header().Set("Content-Type", "application/json")
		_, _ = w.Write([]byte(`{"username":"ok"}`))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	c.UserAgent = ""
	c.Retries = 2 // total attempts = 3
	c.RetryBackoff = 1 * time.Millisecond

	p, err := c.GetPlayer(context.Background(), "ok")
	if err != nil {
		t.Fatalf("expected success after retries, got %v", err)
	}
	if p.Username != "ok" {
		t.Fatalf("unexpected player: %+v", p)
	}
	if attempts != 3 {
		t.Fatalf("expected 3 attempts, got %d", attempts)
	}
}

func TestRetry_429_RetryAfterSeconds(t *testing.T) {
	var attempts int32
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		n := atomic.AddInt32(&attempts, 1)
		if n == 1 {
			w.Header().Set("Retry-After", "0")
			http.Error(w, "rate limited", http.StatusTooManyRequests)
			return
		}
		w.Header().Set("Content-Type", "application/json")
		_, _ = w.Write([]byte(`{"username":"ok"}`))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	c.UserAgent = ""
	c.Retries = 1 // total attempts = 2
	c.RetryBackoff = 1 * time.Millisecond

	_, err := c.GetPlayer(context.Background(), "ok")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if attempts != 2 {
		t.Fatalf("expected 2 attempts, got %d", attempts)
	}
}

func TestNonRetriable4xx_ReturnsHTTPError(t *testing.T) {
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		w.WriteHeader(http.StatusNotFound)
		_, _ = w.Write([]byte("not found"))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	c.Retries = 2
	c.RetryBackoff = 1 * time.Millisecond

	_, err := c.GetPlayer(context.Background(), "nobody")
	if err == nil {
		t.Fatalf("expected error")
	}
	he, ok := err.(*HTTPError)
	if !ok {
		t.Fatalf("expected *HTTPError, got %T: %v", err, err)
	}
	if he.StatusCode != http.StatusNotFound {
		t.Fatalf("unexpected status: %d", he.StatusCode)
	}
	if he.Body == "" {
		t.Fatalf("expected body snippet to be set")
	}
}

func TestListPlayerGameArchives(t *testing.T) {
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.URL.Path != "/player/someone/games/archives" {
			t.Fatalf("unexpected path: %s", r.URL.Path)
		}
		w.Header().Set("Content-Type", "application/json")
		_ = json.NewEncoder(w).Encode(map[string]any{
			"archives": []string{"http://x/a", "http://x/b"},
		})
	}))
	defer ts.Close()

	c := testClientFor(ts)
	got, err := c.ListPlayerGameArchives(context.Background(), "SomeOne")
	if err != nil {
		t.Fatalf("ListPlayerGameArchives error: %v", err)
	}
	if len(got) != 2 || got[0] != "http://x/a" || got[1] != "http://x/b" {
		t.Fatalf("unexpected archives: %#v", got)
	}
}

func TestGetPlayerMonthlyGames_PathAndDecode(t *testing.T) {
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.URL.Path != "/player/someone/games/2023/07" {
			t.Fatalf("unexpected path: %s", r.URL.Path)
		}
		w.Header().Set("Content-Type", "application/json")
		_ = json.NewEncoder(w).Encode(MonthlyGames{Games: []Game{{White: Side{Username: "w"}, Black: Side{Username: "b"}}}})
	}))
	defer ts.Close()

	c := testClientFor(ts)
	mg, err := c.GetPlayerMonthlyGames(context.Background(), "SomeOne", 2023, 7)
	if err != nil {
		t.Fatalf("GetPlayerMonthlyGames error: %v", err)
	}
	if len(mg.Games) != 1 || mg.Games[0].White.Username != "w" || mg.Games[0].Black.Username != "b" {
		t.Fatalf("unexpected monthly games: %+v", mg)
	}
}

func TestGetPlayerMonthlyGames_InvalidMonth(t *testing.T) {
	c := NewClient()
	if _, err := c.GetPlayerMonthlyGames(context.Background(), "u", 2023, 0); err == nil {
		t.Fatalf("expected error for invalid month")
	}
	if _, err := c.GetPlayerMonthlyGames(context.Background(), "u", 2023, 13); err == nil {
		t.Fatalf("expected error for invalid month")
	}
}

func TestGetLeaderboards(t *testing.T) {
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.URL.Path != "/leaderboards" {
			t.Fatalf("unexpected path: %s", r.URL.Path)
		}
		w.Header().Set("Content-Type", "application/json")
		_ = json.NewEncoder(w).Encode(Leaderboards{
			LiveRapid:  []LeaderboardPlayer{{Username: "a", Score: 1}},
			LiveBlitz:  []LeaderboardPlayer{{Username: "b", Score: 2}},
			LiveBullet: []LeaderboardPlayer{{Username: "c", Score: 3}},
			Daily:      []LeaderboardPlayer{{Username: "d", Score: 4}},
		})
	}))
	defer ts.Close()

	c := testClientFor(ts)
	lb, err := c.GetLeaderboards(context.Background())
	if err != nil {
		t.Fatalf("GetLeaderboards error: %v", err)
	}
	if len(lb.LiveRapid) != 1 || lb.LiveRapid[0].Username != "a" {
		t.Fatalf("unexpected leaderboards: %+v", lb)
	}
}

func TestGetPlayerStats(t *testing.T) {
	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		if r.URL.Path != "/player/x/stats" {
			t.Fatalf("unexpected path: %s", r.URL.Path)
		}
		w.Header().Set("Content-Type", "application/json")
		_ = json.NewEncoder(w).Encode(PlayerStats{
			ChessRapid: &StatsCategory{Last: &RatingSample{Rating: 1500}},
		})
	}))
	defer ts.Close()

	c := testClientFor(ts)
	ps, err := c.GetPlayerStats(context.Background(), "X")
	if err != nil {
		t.Fatalf("GetPlayerStats error: %v", err)
	}
	if ps.ChessRapid == nil || ps.ChessRapid.Last == nil || ps.ChessRapid.Last.Rating != 1500 {
		t.Fatalf("unexpected stats: %+v", ps)
	}
}

func TestRetry_429_RetryAfterHTTPDate(t *testing.T) {
	var attempts int32
	// Use an HTTP-date equal to current time to avoid sleeping.
	date := time.Now().UTC().Format(http.TimeFormat)

	ts := httptest.NewServer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		n := atomic.AddInt32(&attempts, 1)
		if n == 1 {
			w.Header().Set("Retry-After", date)
			http.Error(w, "rate limited", http.StatusTooManyRequests)
			return
		}
		w.Header().Set("Content-Type", "application/json")
		_, _ = w.Write([]byte(`{"username":"ok"}`))
	}))
	defer ts.Close()

	c := testClientFor(ts)
	c.UserAgent = ""
	c.Retries = 1
	c.RetryBackoff = 1 * time.Millisecond

	_, err := c.GetPlayer(context.Background(), "ok")
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if attempts != 2 {
		t.Fatalf("expected 2 attempts, got %d", attempts)
	}
}
