INSERT INTO Users (
    email,
    spotify_id,
    spotify_profile_url,
    spotify_username,
    spotify_access_token,
    spotify_refresh_token,
    spotify_expires_at
) VALUES (
    (@email),
    (@spotify_id),
    (@spotify_profile_url),
    (@spotify_username),
    (@spotify_access_token),
    (@spotify_refresh_token),
    (@spotify_expires_at)
)
RETURNING *;