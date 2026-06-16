import {
  Box, Button, Card, CardContent, CircularProgress,
  Grid, Skeleton, Typography,
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import { PieChart, Pie, Cell, Tooltip, Legend } from 'recharts';
import { useDashboardSummary } from '../../hooks/useDashboard';

const STATUS_COLORS = ['#2196f3', '#ff9800', '#9c27b0', '#4caf50', '#9e9e9e'];

export default function DashboardPage() {
  const { data, isLoading, refetch } = useDashboardSummary();

  const pieData = data
    ? [
        { name: 'Open', value: data.ticketsByStatus.new },
        { name: 'In Progress', value: data.ticketsByStatus.inProgress },
        { name: 'Pending', value: data.ticketsByStatus.pending },
        { name: 'Resolved', value: data.ticketsByStatus.resolved },
        { name: 'Closed', value: data.ticketsByStatus.closed },
      ]
    : [];

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5">Dashboard</Typography>
        <Button startIcon={<RefreshIcon />} onClick={() => refetch()} aria-label="Refresh">
          Refresh
        </Button>
      </Box>

      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} sm={6}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom>Leads This Month</Typography>
              {isLoading ? (
                <Skeleton variant="text" width={60} height={48} />
              ) : (
                <Typography variant="h3">{data?.currentMonthLeads ?? 0}</Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom>Customers Converted This Month</Typography>
              {isLoading ? (
                <Skeleton variant="text" width={60} height={48} />
              ) : (
                <Typography variant="h3">{data?.convertedCustomersThisMonth ?? 0}</Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Typography variant="h6" mb={2}>Ticket Status Summary</Typography>
      {isLoading ? (
        <Skeleton variant="rectangular" width={400} height={300} />
      ) : (
        <PieChart width={400} height={300}>
          <Pie data={pieData} dataKey="value" nameKey="name" innerRadius={60} outerRadius={120}>
            {pieData.map((_, i) => (
              <Cell key={i} fill={STATUS_COLORS[i % STATUS_COLORS.length]} />
            ))}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      )}
    </Box>
  );
}
