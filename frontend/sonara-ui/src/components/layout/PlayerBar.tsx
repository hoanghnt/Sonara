import { useRef, useEffect, useState } from "react";
import { usePlayerStore } from "../../stores/playerStore";

const PlayerBar = () => {
  const { currentSong, isPlaying, volume, togglePlay, setVolume } =
    usePlayerStore();

  const audioRef = useRef<HTMLAudioElement | null>(null);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

  useEffect(() => {
    if (!audioRef.current || !currentSong) return;

    audioRef.current.src = `http://localhost:5078/api/songs/${currentSong.id}/stream`;
    audioRef.current.play();
  }, [currentSong]);

  useEffect(() => {
    if (!audioRef.current) return;
    if (isPlaying) {
      audioRef.current.play();
    } else {
      audioRef.current.pause();
    }
  }, [isPlaying]);

  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  const handleTimeUpdate = () => {
    if (audioRef.current) {
      setCurrentTime(audioRef.current.currentTime);
    }
  };

  const handleLoadedMetadata = () => {
    if (audioRef.current) {
      setDuration(audioRef.current.duration);
    }
  };

  const handleSeek = (e: React.ChangeEvent<HTMLInputElement>) => {
    const time = Number(e.target.value);
    if (audioRef.current) {
      audioRef.current.currentTime = time;
      setCurrentTime(time);
    }
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  if (!currentSong) return null;

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-neutral-900 border-t border-neutral-800 px-4 py-3 flex items-center gap-4">
      {/* Song Info */}
      <div className="w-64 flex flex-col">
        <span className="text-sm font-medium truncate">
          {currentSong.title}
        </span>
        <span className="text-xs text-neutral-400 truncate">
          {currentSong.artist}
        </span>
      </div>

      {/* Controls + Progress */}
      <div className="flex-1 flex flex-col items-center gap-1">
        <button
          onClick={togglePlay}
          className="w-8 h-8 bg-white rounded-full flex items-center justify-center hover:scale-105 transition-transform"
        >
          <span className="text-black text-sm">{isPlaying ? "⏸" : "▶"}</span>
        </button>

        <div className="flex items-center gap-2 w-full max-w-lg">
          <span className="text-xs text-neutral-400 w-10 text-right">
            {formatTime(currentTime)}
          </span>
          <input
            type="range"
            min={0}
            max={duration || 0}
            value={currentTime}
            onChange={handleSeek}
            className="flex-1 h-1 accent-green-500"
          />
          <span className="text-xs text-neutral-400 w-10">
            {formatTime(duration)}
          </span>
        </div>
      </div>

      {/* Volume */}
      <div className="w-32 flex items-center gap-2">
        <span className="text-xs">🔊</span>
        <input
          type="range"
          min={0}
          max={1}
          step={0.01}
          value={volume}
          onChange={(e) => setVolume(Number(e.target.value))}
          className="flex-1 h-1 accent-green-500"
        />
      </div>

      <audio
        ref={audioRef}
        onTimeUpdate={handleTimeUpdate}
        onLoadedMetadata={handleLoadedMetadata}
      />
    </div>
  );
};

export default PlayerBar;