import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function ProtectedRoute() {
  const { isAuthenticated, isInitializing } = useAuth();

  if (isInitializing) return null; // prevent flash redirect while rehydrating

  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
}
