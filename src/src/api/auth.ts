import axiosInstance from './axiosInstance';

export interface LoginRequest {
  emailOrUsername: string;
  password: string;
  rememberMe: boolean;
}

export interface LoginUser {
  id: string;
  email: string;
  username: string;
  roles: string[];
}

export interface LoginResponse {
  expiresAt: string; // ISO 8601
  user: LoginUser;
}

export async function loginApi(payload: LoginRequest): Promise<LoginResponse> {
  const { data } = await axiosInstance.post<LoginResponse>('/auth/login', payload);
  return data;
}

export async function logoutApi(): Promise<void> {
  await axiosInstance.post('/auth/logout');
}
