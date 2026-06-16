import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test-setup.ts'],
    server: {
      deps: {
        // MUI imports react-transition-group as a directory — inline it so Vitest resolves correctly
        inline: ['@mui/material', '@mui/icons-material', 'react-transition-group'],
      },
    },
    coverage: {
      provider: 'v8',
      thresholds: { lines: 80, branches: 80, functions: 80, statements: 80 },
    },
  },
});
