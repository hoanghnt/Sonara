import { useState } from "react";
import { playlistService } from "../api/services/playlistService";
import type { Playlist } from "../types/playlist";

interface Props {
  songId: string;
}

const AddToPlaylist = ({ songId }: Props) => {
  const [open, setOpen] = useState(false);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [loaded, setLoaded] = useState(false);
  const [message, setMessage] = useState("");

  const handleOpen = async () => {
    setOpen(!open);
    setMessage("");

    if (!loaded) {
      try {
        const response = await playlistService.getAll();
        setPlaylists(response.data);
        setLoaded(true);
      } catch (error) {
        console.error("Failed to fetch playlists:", error);
      }
    }
  };

  const handleAdd = async (playlistId: string) => {
    try {
      await playlistService.addSong(playlistId, songId);
      setMessage("Added!");
      setTimeout(() => {
        setOpen(false);
        setMessage("");
      }, 1000);
    } catch (error: any) {
      setMessage(error.response?.data || "Failed");
    }
  };

  return (
    <div className="relative">
      <button
        onClick={(e) => {
          e.stopPropagation();
          handleOpen();
        }}
        className="text-neutral-600 hover:text-white transition-colors"
        title="Add to playlist"
      >
        +
      </button>

      {open && (
        <div className="absolute right-0 bottom-8 bg-neutral-800 rounded-md shadow-lg py-2 min-w-48 z-50">
          <p className="px-3 py-1 text-xs text-neutral-400">Add to playlist</p>

          {message ? (
            <p className="px-3 py-2 text-sm text-green-500">{message}</p>
          ) : playlists.length === 0 ? (
            <p className="px-3 py-2 text-sm text-neutral-400">No playlists</p>
          ) : (
            playlists.map((playlist) => (
              <button
                key={playlist.id}
                onClick={(e) => {
                  e.stopPropagation();
                  handleAdd(playlist.id);
                }}
                className="w-full text-left px-3 py-2 text-sm hover:bg-neutral-700 transition-colors"
              >
                {playlist.name}
              </button>
            ))
          )}
        </div>
      )}
    </div>
  );
};

export default AddToPlaylist;