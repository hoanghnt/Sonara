import api from "./api";
import type { Song } from "../../types/song";

export const songService = {
  getAll: () => api.get<Song[]>("/songs"),

  getById: (id: string) => api.get<Song>(`/songs/${id}`),

  search: (keyword: string) =>
    api.get<Song[]>("/songs/search", { params: { keyword } }),

  upload: (formData: FormData) =>
    api.post("/songs", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    }),

  delete: (id: string) => api.delete(`/songs/${id}`),
};