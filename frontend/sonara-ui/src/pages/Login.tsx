import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { authService } from "../api/services/authService";
import { useAuthStore } from "../stores/authStore";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const { login } = useAuthStore();

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/", {replace: true});
    }
  }, [isAuthenticated, navigate]); 

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const response = await authService.login({ email, password });
      login(response.data.accessToken, response.data.refreshToken);
      navigate("/");
    } catch (err: any) {
      setError(err.response?.data?.message || "Login failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-black">
      <form
        onSubmit={handleSubmit}
        className="bg-neutral-900 p-8 rounded-lg w-full max-w-md flex flex-col gap-4"
      >
        <h1 className="text-3xl font-bold text-center">Log in to Sonara</h1>

        {error && (
          <p className="text-red-500 text-sm text-center">{error}</p>
        )}

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="bg-neutral-800 p-3 rounded-md outline-none focus:ring-2 focus:ring-green-500"
          required
        />

        <button
          type="submit"
          disabled={loading}
          className="bg-green-500 text-black font-bold py-3 rounded-full hover:bg-green-400 transition-colors disabled:opacity-50"
        >
          {loading ? "Logging in..." : "Log In"}
        </button>

        <p className="text-neutral-400 text-center text-sm">
          Don't have an account?{" "}
          <Link to="/register" className="text-green-500 hover:underline">
            Sign up
          </Link>
        </p>
      </form>
    </div>
  );
};

export default Login;