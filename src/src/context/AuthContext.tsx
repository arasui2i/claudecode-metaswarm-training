import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import type { LoginUser } from '../api/auth';
import { logoutApi } from '../api/auth';

interface AuthState {
  user: LoginUser | null;
  isAuthenticated: boolean;
  isInitializing: boolean;
  login: (user: LoginUser) => void;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<LoginUser | null>(null);
  const [isInitializing, setIsInitializing] = useState(true);

  useEffect(() => {
    // Rehydrate from sessionStorage (user profile only — token is in HttpOnly cookie)
    try {
      const stored = sessionStorage.getItem('crm_user');
      if (stored) setUser(JSON.parse(stored) as LoginUser);
    } catch {
      // ignore malformed storage
    } finally {
      setIsInitializing(false);
    }
  }, []);

  const login = (loggedInUser: LoginUser) => {
    setUser(loggedInUser);
    sessionStorage.setItem('crm_user', JSON.stringify(loggedInUser));
  };

  const logout = async () => {
    await logoutApi();
    setUser(null);
    sessionStorage.removeItem('crm_user');
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, isInitializing, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthState {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
