import { defineConfig } from 'vitest/config';
import { VERSION } from '@angular/cli';

export default defineConfig({
  test: {
    coverage: {
      excludeAfterRemap: true,
      reportsDirectory: 'coverage/frontend',
      enabled: false,
      thresholds: {},
      watermarks: {},
    },
    projects: [
      {
        test: {
          setupFiles: ['init-testbed.js'],
          globals: true,
          isolate: false,
          sequence: {
            setupFiles: 'list',
          },
          root: `.angular/cache/${VERSION.full}/frontend/unit-test/output-files`,
          include: ['spec-*.js'],
          environment: 'jsdom',
        },
        optimizeDeps: {
          noDiscovery: true,
          include: [
            '@angular/core',
            '@angular/core/testing',
            '@angular/platform-browser/testing',
            '@angular/router',
          ],
        },
        resolve: {
          mainFields: ['es2020', 'module', 'main'],
          conditions: ['es2015', 'es2020', 'module', 'node', 'development|production'],
        },
        plugins: [
          {
            name: 'angular:test-in-memory-provider',
            enforce: 'pre',
          },
          {
            name: 'angular:html-index',
          },
        ],
      },
    ],
  },
});
