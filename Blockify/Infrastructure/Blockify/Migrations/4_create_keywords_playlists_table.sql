CREATE TABLE IF NOT EXISTS Playlists (
    id TEXT PRIMARY KEY,
    owner_id TEXT REFERENCES Users(spotify_id) ON DELETE CASCADE,
    spotify_id TEXT NOT NULL,
);  