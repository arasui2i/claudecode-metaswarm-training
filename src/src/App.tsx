import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/Login/LoginPage';
import CustomerListPage from './pages/Customers/CustomerListPage';
import LeadListPage from './pages/Leads/LeadListPage';
import ContactListPage from './pages/Contacts/ContactListPage';
import AccountListPage from './pages/Accounts/AccountListPage';
import OpportunityListPage from './pages/Opportunities/OpportunityListPage';
import ActivityListPage from './pages/activities/ActivityListPage';

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
              <Route path="/contacts" element={<ContactListPage />} />
              <Route path="/accounts" element={<AccountListPage />} />
              <Route path="/opportunities" element={<OpportunityListPage />} />
              <Route path="/activities" element={<ActivityListPage />} />
            </Route>
            <Route path="/" element={<Navigate to="/customers" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
