CREATE TABLE IF NOT EXISTS Users (
    id BIGSERIAL PRIMARY KEY,
    email CITEXT UNIQUE NOT NULL,
    spotify_id VARCHAR(255) UNIQUE NOT NULL,
    spotify_profile_url VARCHAR(255) NOT NULL,
    spotify_username VARCHAR(255) NOT NULL,
    spotify_access_token TEXT NOT NULL,
    spotify_refresh_token TEXT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = NOW();
   RETURN NEW;
END;
$$ language 'plpgsql';

DROP TRIGGER IF EXISTS set_updated_at ON Users;
CREATE TRIGGER set_updated_at
BEFORE UPDATE ON Users
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
