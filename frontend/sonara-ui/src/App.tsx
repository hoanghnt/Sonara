import { useEffect } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./components/layout/MainLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import { useAuthStore } from "./stores/authStore";
import Upload from "./pages/Upload";
import PlaylistDetail from "./pages/PlaylistDetail";
import Search from "./pages/Search";
import Playlists from "./pages/Playlists";

const App = () => {
  const { loadToken } = useAuthStore();

  useEffect(() => {
    loadToken();
  }, [loadToken]);

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route element={<ProtectedRoute />}>
          <Route element={<MainLayout />}>
            <Route path="/" element={<Home />} />
            <Route path="/upload" element={<Upload />} />
            <Route path="/search" element={<Search />} />
            <Route path="/playlists" element={<Playlists />} />
            <Route path="/playlists/:id" element={<PlaylistDetail />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
