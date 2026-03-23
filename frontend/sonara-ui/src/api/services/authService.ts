import api from "./api";

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export const authService = {
  register: (data: RegisterRequest) =>
    api.post<AuthResponse>("/auth/register", data),

  login: (data: LoginRequest) =>
    api.post<AuthResponse>("/auth/login", data),

  refreshToken: (refreshToken: string) =>
    api.post<AuthResponse>("/auth/refresh", { refreshToken }),
};