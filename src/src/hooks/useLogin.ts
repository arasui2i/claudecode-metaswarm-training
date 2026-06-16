import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { loginApi, type LoginRequest } from '../api/auth';
import { useAuth } from '../context/AuthContext';

export default function useLogin() {
  const { login } = useAuth();
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (payload: LoginRequest) => loginApi(payload),
    onSuccess: (data) => {
      login(data.user);
      navigate('/customers');
    },
  });
}
