INSERT INTO Playlists (id, owner_id, spotify_id, keyword)
VALUES ((@id), (@userId), (@spotifyId), (@keyword))
RETURNING *;