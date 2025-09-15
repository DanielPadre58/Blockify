SELECT spotify_access_token, spotify_expires_at, spotify_refresh_token
FROM Users
WHERE id = (@userId);