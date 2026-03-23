import { Link } from "react-router-dom";

const Sidebar = () => {
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
      </nav>
    </aside>
  );
};

export default Sidebar;