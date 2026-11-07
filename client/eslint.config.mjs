import globals from 'globals';

// ─── KNOWN LIMITATION ────────────────────────────────────────────────
// typescript-eslint@8.x does not support TS 7.0 (upstream #10940).
// eslint-config-next imports typescript-eslint internally, so it crashes too.
// Until TS 7.1 + typescript-eslint alignment, lint only covers .mjs/.js config
// files. TypeScript correctness is enforced via `npx tsc --noEmit` (Phase gate).
//
// Once typescript-eslint adds TS 7 support, restore:
//   import { FlatCompat } from '@eslint/eslintrc';
//   ...compat.extends('next/core-web-vitals', 'next/typescript'),
// ────────────────────────────────────────────────────────────────────

export default [
  {
    // Ignore all TS/TSX and generated dirs
    ignores: [
      '.next/',
      'node_modules/',
      'next-env.d.ts',
      'out/',
      'build/',
      '**/*.ts',
      '**/*.tsx',
    ],
  },
  {
    files: ['**/*.{js,mjs}'],
    languageOptions: {
      ecmaVersion: 'latest',
      sourceType: 'module',
      globals: { ...globals.node },
    },
    rules: {
      'no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
    },
  },
];
