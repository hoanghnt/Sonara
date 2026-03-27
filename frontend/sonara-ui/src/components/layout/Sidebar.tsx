import { Link, useNavigate } from "react-router-dom";
import { useAuthStore } from "../../stores/authStore";

const Sidebar = () => {
  const navigate = useNavigate();
  const logout = useAuthStore((state) => state.logout);
  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <aside className="w-64 bg-neutral-950 p-6 flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-green-500">Sonara</h1>

      <nav className="flex flex-col gap-2">
        <Link to="/" className="hover:text-green-400 transition-colors">
          Home
        </Link>
        <Link to="/search" className="hover:text-green-400 transition-colors">
          Search
        </Link>
        <Link
          to="/playlists"
          className="hover:text-green-400 transition-colors"
        >
          Playlists
        </Link>
        <Link to="/upload" className="hover:text-green-400 transition-colors">
          Upload
        </Link>
        <button
          type="button"
          onClick={handleLogout}
          className="text-left hover:text-green-400 transition-colors"
        >
          Log out
        </button>
      </nav>
    </aside>
  );
};

export default Sidebar;
