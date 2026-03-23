import { useState } from "react";
import { songService } from "../api/services/songService";
import { usePlayerStore } from "../stores/playerStore";
import type { Song } from "../types/song";
import AddToPlaylist from "../components/AddToPlaylist";

const Search = () => {
  const [keyword, setKeyword] = useState("");
  const [results, setResults] = useState<Song[]>([]);
  const [searched, setSearched] = useState(false);
  const [loading, setLoading] = useState(false);

  const { playSong, currentSong, togglePlay } = usePlayerStore();

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!keyword.trim()) return;

    setLoading(true);
    try {
      const response = await songService.search(keyword);
      setResults(response.data);
      setSearched(true);
    } catch (error) {
      console.error("Search failed:", error);
    } finally {
      setLoading(false);
    }
  };

  const handlePlay = (song: Song) => {
    if (currentSong?.id === song.id) {
      togglePlay();
    } else {
      playSong({ id: song.id, title: song.title, artist: song.artist, duration: song.duration });
    }
  };

  const formatDuration = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  return (
    <div className="p-8">
      <h2 className="text-3xl font-bold mb-6">Search</h2>

      <form onSubmit={handleSearch} className="flex gap-2 mb-6">
        <input
          type="text"
          placeholder="Search by title or artist..."
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          className="flex-1 bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
        />
        <button
          type="submit"
          disabled={loading}
          className="bg-green-500 text-black font-bold px-6 rounded-full hover:bg-green-400 transition-colors disabled:opacity-50"
        >
          {loading ? "..." : "Search"}
        </button>
      </form>

      {searched && results.length === 0 && (
        <p className="text-neutral-400">No results found for "{keyword}"</p>
      )}

      {results.length > 0 && (
        <table className="w-full text-left">
          <thead>
            <tr className="text-neutral-400 text-sm border-b border-neutral-800">
              <th className="pb-2 w-12">#</th>
              <th className="pb-2">Title</th>
              <th className="pb-2">Artist</th>
              <th className="pb-2">Album</th>
              <th className="pb-2 text-right">Duration</th>
            </tr>
          </thead>
          <tbody>
            {results.map((song, index) => (
              <tr
                key={song.id}
                onClick={() => handlePlay(song)}
                className="hover:bg-neutral-800 cursor-pointer transition-colors"
              >
                <td className="py-3 text-neutral-400">{index + 1}</td>
                <td className="py-3">
                  <span className={currentSong?.id === song.id ? "text-green-500" : ""}>
                    {song.title}
                  </span>
                </td>
                <td className="py-3 text-neutral-400">{song.artist}</td>
                <td className="py-3 text-neutral-400">{song.album}</td>
                <td className="py-3 text-neutral-400 text-right">
                  {formatDuration(song.duration)}
                </td>
                <AddToPlaylist songId={song.id} />
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default Search;