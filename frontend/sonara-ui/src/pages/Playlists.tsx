import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { playlistService } from "../api/services/playlistService";
import type { Playlist } from "../types/playlist";

const Playlists = () => {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const fetchPlaylists = async () => {
    try {
      const response = await playlistService.getAll();
      setPlaylists(response.data);
    } catch (error) {
      console.error("Failed to fetch playlists:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlaylists();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await playlistService.create({ name, description });
      setName("");
      setDescription("");
      setShowForm(false);
      fetchPlaylists();
    } catch (error) {
      console.error("Failed to create playlist:", error);
    }
  };

  if (loading) {
    return <div className="p-8 text-neutral-400">Loading playlists...</div>;
  }

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-3xl font-bold">My Playlists</h2>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-green-500 text-black font-bold px-4 py-2 rounded-full hover:bg-green-400 transition-colors"
        >
          {showForm ? "Cancel" : "+ New Playlist"}
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="flex gap-2 mb-6">
          <input
            type="text"
            placeholder="Playlist name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="flex-1 bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
            required
          />
          <input
            type="text"
            placeholder="Description (optional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="flex-1 bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
          />
          <button
            type="submit"
            className="bg-green-500 text-black font-bold px-6 rounded-full hover:bg-green-400 transition-colors"
          >
            Create
          </button>
        </form>
      )}

      {playlists.length === 0 ? (
        <p className="text-neutral-400">No playlists yet. Create your first one!</p>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {playlists.map((playlist) => (
            <Link
              key={playlist.id}
              to={`/playlists/${playlist.id}`}
              className="bg-neutral-900 p-4 rounded-lg hover:bg-neutral-800 transition-colors"
            >
              <h3 className="font-bold truncate">{playlist.name}</h3>
              <p className="text-sm text-neutral-400 truncate">
                {playlist.description || "No description"}
              </p>
              <p className="text-xs text-neutral-500 mt-2">
                {playlist.songs.length} songs
              </p>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
};

export default Playlists;