module.exports = [
  {
    ignores: [
      'wwwroot/lib/**',  // ✅ Ignore Bootstrap, jQuery, other vendor files
      'node_modules/**',  // ✅ Ignore node_modules if it exists
    ],
    files: ['wwwroot/js/**/*.js'],  // ✅ Lint only the JS files
    languageOptions: {
      ecmaVersion: 2021,
      sourceType: 'script',
    },
    rules: {
      semi: ['error', 'always'],
      quotes: ['error', 'single'],
      'no-unused-vars': 'warn',
      'no-console': 'off',
      eqeqeq: ['warn', 'always'],
      curly: 'warn',
    },
  },
];
