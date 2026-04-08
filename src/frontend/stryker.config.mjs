/** @type {import('@stryker-mutator/core').PartialStrykerOptions} */
export default {
  packageManager: 'npm',
  reporters: ['html', 'clear-text', 'progress'],
  testRunner: 'vitest',
  vitest: { related: false },
  coverageAnalysis: 'perTest',
  buildCommand: 'ng test --dump-virtual-files --watch=false',
};
