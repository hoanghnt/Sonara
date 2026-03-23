import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { songService } from "../api/services/songService";

const Upload = () => {
  const [title, setTitle] = useState("");
  const [artist, setArtist] = useState("");
  const [album, setAlbum] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [coverImage, setCoverImage] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file) {
      setError("Please select an audio file");
      return;
    }

    setError("");
    setLoading(true);

    try {
      const formData = new FormData();
      formData.append("title", title);
      formData.append("artist", artist);
      formData.append("album", album);
      formData.append("file", file);
      if (coverImage) {
        formData.append("coverImage", coverImage);
      }

      await songService.upload(formData);
      navigate("/");
    } catch (err: any) {
      setError(err.response?.data || "Upload failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-8 max-w-lg">
      <h2 className="text-3xl font-bold mb-6">Upload Song</h2>

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        {error && <p className="text-red-500 text-sm">{error}</p>}

        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
          required
        />

        <input
          type="text"
          placeholder="Artist"
          value={artist}
          onChange={(e) => setArtist(e.target.value)}
          className="bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
          required
        />

        <input
          type="text"
          placeholder="Album"
          value={album}
          onChange={(e) => setAlbum(e.target.value)}
          className="bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
        />

        <div>
          <label className="block text-sm text-neutral-400 mb-1">
            Audio File (MP3, FLAC)
          </label>
          <input
            type="file"
            accept="audio/*"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
            className="text-sm text-neutral-400"
            required
          />
        </div>

        <div>
          <label className="block text-sm text-neutral-400 mb-1">
            Cover Image (optional)
          </label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setCoverImage(e.target.files?.[0] || null)}
            className="text-sm text-neutral-400"
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          className="bg-green-500 text-black font-bold py-3 rounded-full hover:bg-green-400 transition-colors disabled:opacity-50"
        >
          {loading ? "Uploading..." : "Upload"}
        </button>
      </form>
    </div>
  );
};

export default Upload;