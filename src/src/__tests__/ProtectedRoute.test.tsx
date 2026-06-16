import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import ProtectedRoute from '../components/ProtectedRoute';

vi.mock('../context/AuthContext', () => ({
  useAuth: vi.fn(),
}));

import { useAuth } from '../context/AuthContext';

describe('ProtectedRoute', () => {
  it('redirects to /login when not authenticated', () => {
    vi.mocked(useAuth).mockReturnValue({
      isAuthenticated: false, isInitializing: false,
      user: null, login: vi.fn(), logout: vi.fn(),
    });

    render(
      <MemoryRouter initialEntries={['/customers']}>
        <Routes>
          <Route path="/login" element={<div>Login Page</div>} />
          <Route element={<ProtectedRoute />}>
            <Route path="/customers" element={<div>Customers</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByText('Login Page')).toBeInTheDocument();
  });

  it('renders children when authenticated', () => {
    vi.mocked(useAuth).mockReturnValue({
      isAuthenticated: true, isInitializing: false,
      user: { id: '1', email: 'a@b.com', username: 'ab', roles: ['Admin'] },
      login: vi.fn(), logout: vi.fn(),
    });

    render(
      <MemoryRouter initialEntries={['/customers']}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/customers" element={<div>Customers</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByText('Customers')).toBeInTheDocument();
  });
});
