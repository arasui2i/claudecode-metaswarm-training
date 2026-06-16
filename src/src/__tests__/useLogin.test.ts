import { renderHook, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import useLogin from '../hooks/useLogin';
import * as authApi from '../api/auth';

vi.mock('../api/auth');
vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ login: vi.fn() }),
}));
vi.mock('react-router-dom', () => ({
  useNavigate: () => vi.fn(),
}));

const wrapper = ({ children }: { children: React.ReactNode }) => {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: queryClient }, children);
};

describe('useLogin', () => {
  beforeEach(() => vi.clearAllMocks());

  it('calls loginApi with correct payload', async () => {
    vi.mocked(authApi.loginApi).mockResolvedValueOnce({
      expiresAt: new Date().toISOString(),
      user: { id: '1', email: 'a@b.com', username: 'ab', roles: ['Admin'] },
    });

    const { result } = renderHook(() => useLogin(), { wrapper });
    result.current.mutate({ emailOrUsername: 'a@b.com', password: 'password123', rememberMe: false });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(authApi.loginApi).toHaveBeenCalledWith({
      emailOrUsername: 'a@b.com', password: 'password123', rememberMe: false,
    });
  });

  it('exposes error message on 401', async () => {
    vi.mocked(authApi.loginApi).mockRejectedValueOnce({ response: { status: 401 } });

    const { result } = renderHook(() => useLogin(), { wrapper });
    result.current.mutate({ emailOrUsername: 'x@x.com', password: 'wrongpass', rememberMe: false });

    await waitFor(() => expect(result.current.isError).toBe(true));
  });
});
