UPDATE Users
SET spotify_access_token = (@accessToken),
    spotify_refresh_token = (@refreshToken)
WHERE id = (@userId);