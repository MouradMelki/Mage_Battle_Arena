---
events:
  - "file_write"
  - "file_edit"
---
# Safety Engine, Anti-Regression & Pre-Flight Validation

## 1. Feature Preservation Mandate
- **No Silent Modification:** You are strictly barred from removing, altering, or deprecating existing, functional features unless the user's prompt explicitly requests a change or removal of that exact feature.
- **Dependency Scan:** Before committing any file change, check the parent component structure to guarantee your updates will not interrupt or crash data pipelines utilized by other views.

## 2. Pre-Flight Verification Loop
- **The Zero-Error Threshold:** Before reporting any change task complete, you must run the local verification suite directly inside the workspace terminal:
```bash
  npm run build && npm run lint
