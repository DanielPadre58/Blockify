UPDATE Users
SET spotify_access_token = (@accessToken),
    spotify_refresh_token = (@refreshToken),
    spotify_expires_at = (@expiresAt)
WHERE id = (@userId);