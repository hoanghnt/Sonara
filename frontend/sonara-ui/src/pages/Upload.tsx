import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { songService } from "../api/services/songService";
import axios from "axios";

const MAX_AUDIO_BYTES = 50 * 1024 * 1024;
const MAX_COVER_BYTES = 5 * 1024 * 1024;

function formatBytes(n: number): string {
  if (n < 1024) return `${n} B`;
  if (n < 1024 * 1024) return `${(n / 1024).toFixed(1)} KB`;
  return `${(n / (1024 * 1024)).toFixed(1)} MB`;
}

function uploadErrorMessage(err: unknown): string {
  if (!axios.isAxiosError(err) || !err.response) {
    return "Upload failed. Check your connection.";
  }
  const { status, data } = err.response;
  if (status === 413) {
    return `Payload too large for server (max about ${formatBytes(MAX_AUDIO_BYTES)} audio + small cover). Try a smaller file.`;
  }
  if (typeof data === "string") {
    const trimmed = data.trim();
    if (trimmed.startsWith("<")) {
      return "Server rejected the upload. If the file is large, try a smaller one.";
    }
    return trimmed;
  }
  return "Upload failed.";
}

const Upload = () => {
  const [title, setTitle] = useState("");
  const [artist, setArtist] = useState("");
  const [album, setAlbum] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [coverImage, setCoverImage] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const onAudioPicked = (f: File | null) => {
    setError("");
    if (!f) {
      setFile(null);
      return;
    }
    if (f.size > MAX_AUDIO_BYTES) {
      setError(
        `Audio must be at most ${formatBytes(MAX_AUDIO_BYTES)} (selected: ${formatBytes(f.size)}).`,
      );
      setFile(null);
      return;
    }
    setFile(f);
  };
  const onCoverPicked = (f: File | null) => {
    setError("");
    if (!f) {
      setCoverImage(null);
      return;
    }
    if (f.size > MAX_COVER_BYTES) {
      setError(`Cover must be at most ${formatBytes(MAX_COVER_BYTES)}.`);
      setCoverImage(null);
      return;
    }
    setCoverImage(f);
  };

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
    } catch (err) {
      setError(uploadErrorMessage(err));
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
            Audio (max {formatBytes(MAX_AUDIO_BYTES)})
          </label>
          <input
            type="file"
            accept="audio/*"
            onChange={(e) => onAudioPicked(e.target.files?.[0] ?? null)}
            className="text-sm text-neutral-400"
            required
          />
        </div>
        <div>
          <label className="block text-sm text-neutral-400 mb-1">
            Cover (optional, max {formatBytes(MAX_COVER_BYTES)})
          </label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => onCoverPicked(e.target.files?.[0] ?? null)}
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
