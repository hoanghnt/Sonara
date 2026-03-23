import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { playlistService } from "../api/services/playlistService";
import { usePlayerStore } from "../stores/playerStore";
import type { Playlist } from "../types/playlist";
import type { Song } from "../types/song";

const PlaylistDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [playlist, setPlaylist] = useState<Playlist | null>(null);
  const [loading, setLoading] = useState(true);
  const { playSong, currentSong, togglePlay } = usePlayerStore();

  const fetchPlaylist = async () => {
    try {
      const response = await playlistService.getById(id!);
      setPlaylist(response.data);
    } catch (error) {
      console.error("Failed to fetch playlist:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlaylist();
  }, [id]);

  const handlePlay = (song: Song) => {
    if (currentSong?.id === song.id) {
      togglePlay();
    } else {
      playSong({ id: song.id, title: song.title, artist: song.artist, duration: song.duration });
    }
  };

  const handleRemoveSong = async (songId: string) => {
    try {
      await playlistService.removeSong(id!, songId);
      fetchPlaylist();
    } catch (error) {
      console.error("Failed to remove song:", error);
    }
  };

  const handleDelete = async () => {
    if (!confirm("Delete this playlist?")) return;
    try {
      await playlistService.delete(id!);
      navigate("/playlists");
    } catch (error) {
      console.error("Failed to delete playlist:", error);
    }
  };

  const formatDuration = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  if (loading) {
    return <div className="p-8 text-neutral-400">Loading...</div>;
  }

  if (!playlist) {
    return <div className="p-8 text-neutral-400">Playlist not found</div>;
  }

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h2 className="text-3xl font-bold">{playlist.name}</h2>
          <p className="text-neutral-400">{playlist.description}</p>
          <p className="text-sm text-neutral-500 mt-1">
            {playlist.songs.length} songs
          </p>
        </div>
        <button
          onClick={handleDelete}
          className="text-red-500 hover:text-red-400 transition-colors"
        >
          Delete Playlist
        </button>
      </div>

      {playlist.songs.length === 0 ? (
        <p className="text-neutral-400">
          This playlist is empty. Add songs from the Home page!
        </p>
      ) : (
        <table className="w-full text-left">
          <thead>
            <tr className="text-neutral-400 text-sm border-b border-neutral-800">
              <th className="pb-2 w-12">#</th>
              <th className="pb-2">Title</th>
              <th className="pb-2">Artist</th>
              <th className="pb-2 text-right">Duration</th>
              <th className="pb-2 w-12"></th>
            </tr>
          </thead>
          <tbody>
            {playlist.songs.map((song, index) => (
              <tr
                key={song.id}
                className="hover:bg-neutral-800 transition-colors group"
              >
                <td
                  className="py-3 text-neutral-400 cursor-pointer"
                  onClick={() => handlePlay(song)}
                >
                  {index + 1}
                </td>
                <td
                  className="py-3 cursor-pointer"
                  onClick={() => handlePlay(song)}
                >
                  <span className={currentSong?.id === song.id ? "text-green-500" : ""}>
                    {song.title}
                  </span>
                </td>
                <td className="py-3 text-neutral-400">{song.artist}</td>
                <td className="py-3 text-neutral-400 text-right">
                  {formatDuration(song.duration)}
                </td>
                <td className="py-3">
                  <button
                    onClick={() => handleRemoveSong(song.id)}
                    className="text-neutral-600 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-all"
                  >
                    ✕
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default PlaylistDetail;