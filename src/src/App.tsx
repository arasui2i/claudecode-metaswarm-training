import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/Login/LoginPage';
import CustomerListPage from './pages/Customers/CustomerListPage';
import LeadListPage from './pages/Leads/LeadListPage';

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/forgot-password" element={<p>Forgot password — coming soon</p>} />
            <Route element={<ProtectedRoute />}>
              <Route path="/customers" element={<CustomerListPage />} />
              <Route path="/leads" element={<LeadListPage />} />
            </Route>
            <Route path="/" element={<Navigate to="/customers" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
