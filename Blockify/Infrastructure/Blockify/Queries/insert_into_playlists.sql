INSERT INTO Playlists (id, owner_id, spotify_id)
VALUES ((@id), (@userId), (@spotifyId))
RETURNING *;