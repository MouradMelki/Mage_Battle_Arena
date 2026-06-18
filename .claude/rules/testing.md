---
paths:
  - "**/Tests/**/*.cs"
  - "**/*Test.cs"
  - "**/*Tests.cs"
---
# Unity Test Framework & Deterministic Mocking Protocols

## 1. Structural Validation Mechanics
- **EditMode Separation:** Pure framework calculations, math arrays, and data state systems must run inside decoupled EditMode unit test frameworks. These must execute instantly without loading scene entities.
- **PlayMode Component Integration:** Frame-dependent structural pipelines, physics interactions, and visual layout sequences must operate under isolated PlayMode tests utilizing clean test scene templates.

## 2. Mocking & Framework Isolation
- **No Real System Side-Effects:** Mock out all concrete implementations handling external file system storage saves, live servers, or Unity Gaming Services APIs using interface abstraction injections.
- **Deterministic Seeding:** Completely ban dynamic random parameters (`UnityEngine.Random`) inside assertions. Seed input parameters explicitly to ensure reproducible test sweeps across all local pipelines and continuous integration matrices.