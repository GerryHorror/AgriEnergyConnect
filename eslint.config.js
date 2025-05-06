export default [
  {
    files: ['**/*.js'],
    languageOptions: {
      ecmaVersion: 2021,
      sourceType: 'script', 
    },
    rules: {
      semi: ['error', 'always'],           // enforce semicolons
      quotes: ['error', 'single'],         // enforce single quotes
      'no-unused-vars': 'warn',            // warn on unused variables
      'no-console': 'off',                 // allow console.log for debugging
      'eqeqeq': ['warn', 'always'],        // warn if not using ===
      curly: 'warn',                       // warn if missing curly braces
    },
  },
];
