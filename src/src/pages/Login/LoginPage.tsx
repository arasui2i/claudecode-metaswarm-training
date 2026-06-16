import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import {
  Box, Button, Checkbox, CircularProgress, FormControlLabel,
  Grid, IconButton, InputAdornment, TextField, Typography,
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import useLogin from '../../hooks/useLogin';
import type { LoginRequest } from '../../api/auth';

export default function LoginPage() {
  const [showPassword, setShowPassword] = useState(false);
  const { mutate: login, isPending, isError, error } = useLogin();

  const { register, handleSubmit, formState: { errors } } = useForm<LoginRequest>({
    defaultValues: { emailOrUsername: '', password: '', rememberMe: false },
  });

  const onSubmit = (data: LoginRequest) => login(data);

  const apiError = isError
    ? 'Invalid email/username or password.'
    : null;

  return (
    <Grid container sx={{ minHeight: '100vh' }}>
      {/* Left panel — illustration placeholder */}
      <Grid size={{ xs: false, md: 6 }}
        sx={{ bgcolor: 'primary.main', display: { xs: 'none', md: 'flex' },
              alignItems: 'center', justifyContent: 'center' }}>
        <Typography variant="h4" color="white">Anticlock CRM</Typography>
      </Grid>

      {/* Right panel — login form */}
      <Grid size={{ xs: 12, md: 6 }}
        sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', p: 4 }}>
        <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ width: '100%', maxWidth: 400 }}>
          <Typography variant="h4" gutterBottom>Welcome Back :)</Typography>
          <Typography variant="body2" color="text.secondary" mb={3}>
            Login with your email and password
          </Typography>

          <TextField
            fullWidth label="Email Address" type="email" margin="normal"
            inputProps={{ 'aria-label': 'Email Address' }}
            error={!!errors.emailOrUsername}
            helperText={errors.emailOrUsername ? 'Email is required' : ''}
            {...register('emailOrUsername', { required: true })}
          />

          <TextField
            fullWidth label="Password" margin="normal"
            type={showPassword ? 'text' : 'password'}
            inputProps={{ 'aria-label': 'Password' }}
            error={!!errors.password}
            helperText={
              errors.password?.type === 'required' ? 'Password is required'
              : errors.password?.type === 'minLength' ? 'At least 8 characters required'
              : ''
            }
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={() => setShowPassword(p => !p)} edge="end"
                    aria-label="toggle password visibility">
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            }}
            {...register('password', { required: true, minLength: 8 })}
          />

          <FormControlLabel
            control={<Checkbox {...register('rememberMe')} />}
            label="Remember Me"
            inputRef={register('rememberMe').ref}
            sx={{ mt: 1 }}
          />

          {apiError && (
            <Typography color="error" variant="body2" mt={1}>{apiError}</Typography>
          )}

          <Button fullWidth type="submit" variant="contained" size="large"
            sx={{ mt: 3 }} disabled={isPending}>
            {isPending ? <CircularProgress size={24} color="inherit" /> : 'Login Now'}
          </Button>

          <Box mt={2} textAlign="center">
            <Link to="/forgot-password" style={{ textDecoration: 'none' }}>
              <Typography variant="body2" color="primary">Forgot Password?</Typography>
            </Link>
          </Box>
        </Box>
      </Grid>
    </Grid>
  );
}
