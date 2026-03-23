import api from "./api";
import type { Playlist } from "../../types/playlist";

export interface CreatePlaylistRequest {
  name: string;
  description: string;
}

export interface UpdatePlaylistRequest {
  name: string;
  description: string;
}

export const playlistService = {
  getAll: () => api.get<Playlist[]>("/playlists"),

  getById: (id: string) => api.get<Playlist>(`/playlists/${id}`),

  create: (data: CreatePlaylistRequest) => api.post("/playlists", data),

  update: (id: string, data: UpdatePlaylistRequest) =>
    api.put(`/playlists/${id}`, data),

  delete: (id: string) => api.delete(`/playlists/${id}`),

  addSong: (playlistId: string, songId: string) =>
    api.post(`/playlists/${playlistId}/songs/${songId}`),

  removeSong: (playlistId: string, songId: string) =>
    api.delete(`/playlists/${playlistId}/songs/${songId}`),
};