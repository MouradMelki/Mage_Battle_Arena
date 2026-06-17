# Product Specifications & Requirement Processing

## 1. Requirement Ingestion Protocol
- **No Intuitive Guesswork:** When a feature document from the `specs/` directory is referenced, you must treat every User Story and Acceptance Criterion as an absolute, immutable technical constraint. Do not invent functionality outside the documented scope.
- **Deconstruction First:** Before generating code for a new feature spec, map out the implementation steps within your internal `<thinking_scratchpad>` block. Classify tasks into: Core State/Database Changes, Backend API/Server Actions, and UI/UX Component Layers.

## 2. Acceptance Criteria (AC) Verification
- **Explicit Mapping:** Every single piece of code written must explicitly tie back to a documented Acceptance Criterion in the active feature spec file. 
- **Definition of Done (DoD):** A feature spec is considered implemented only when:
  1. Every functional Acceptance Criterion passes manual or unit validation.
  2. The code passes the standard build and lint verification loops (`npm run build && npm run lint`).
  3. No legacy or adjacent features show visual or structural regression.
