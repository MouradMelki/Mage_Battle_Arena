---
paths:
  - "**/*.test.ts"
  - "**/*.test.tsx"
  - "**/__tests__/**/*"
---
# Testing Framework & Isolated Mocking Protocols

## 1. Testing Technologies
- **Unit & Component Integration:** Vitest paired with React Testing Library.
- **End-to-End Delivery:** Playwright.

## 2. Total Network & DB Isolation
- **No External Network Hits:** All network endpoints, internal server data routes, or external SDK functions must be strictly intercepted and mocked utilizing standard mocking frameworks (e.g., MSW or explicit Vitest spy frameworks).
- **No Database Side Effects:** Testing runs must never execute changes against a live development or staging database instance. All dynamic state must use clean mock seeds.

## 3. Structural Accuracy
- **Deterministic Bounds:** Completely ban unstable runtime hooks like raw `new Date()` or fluctuating random IDs inside assertions. Seed all test variables to guarantee perfect, repeatable build results.
