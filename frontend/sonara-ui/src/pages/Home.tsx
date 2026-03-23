import { useEffect, useState } from "react";
import { songService } from "../api/services/songService";
import { usePlayerStore } from "../stores/playerStore";
import type { Song } from "../types/song";
import AddToPlaylist from "../components/AddToPlaylist";

const Home = () => {
  const [songs, setSongs] = useState<Song[]>([]);
  const [loading, setLoading] = useState(true);
  const { playSong, currentSong, isPlaying, togglePlay } = usePlayerStore();

  useEffect(() => {
    const fetchSongs = async () => {
      try {
        const response = await songService.getAll();
        setSongs(response.data);
      } catch (error) {
        console.error("Failed to fetch songs:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchSongs();
  }, []);

  const formatDuration = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  const handlePlay = (song: Song) => {
    if (currentSong?.id === song.id) {
      togglePlay();
    } else {
      playSong({
        id: song.id,
        title: song.title,
        artist: song.artist,
        duration: song.duration,
      });
    }
  };

  if (loading) {
    return <div className="p-8 text-neutral-400">Loading songs...</div>;
  }

  return (
    <div className="p-8">
      <h2 className="text-3xl font-bold mb-6">All Songs</h2>

      {songs.length === 0 ? (
        <p className="text-neutral-400">
          No songs yet. Upload your first song!
        </p>
      ) : (
        <table className="w-full text-left">
          <thead>
            <tr className="text-neutral-400 text-sm border-b border-neutral-800">
              <th className="pb-2 w-12">#</th>
              <th className="pb-2">Title</th>
              <th className="pb-2">Artist</th>
              <th className="pb-2">Album</th>
              <th className="pb-2 text-right">Duration</th>
              <th className="pb-2 w-12"></th>
            </tr>
          </thead>
          <tbody>
            {songs.map((song, index) => (
              <tr
                key={song.id}
                onClick={() => handlePlay(song)}
                className="hover:bg-neutral-800 cursor-pointer transition-colors group"
              >
                <td className="py-3 text-neutral-400">{index + 1}</td>
                <td className="py-3">
                  <span
                    className={
                      currentSong?.id === song.id ? "text-green-500" : ""
                    }
                  >
                    {song.title}
                  </span>
                </td>
                <td className="py-3 text-neutral-400">{song.artist}</td>
                <td className="py-3 text-neutral-400">{song.album}</td>
                <td className="py-3 text-neutral-400 text-right">
                  {formatDuration(song.duration)}
                </td>
                <td className="py-3">
                  <AddToPlaylist songId={song.id} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default Home;
