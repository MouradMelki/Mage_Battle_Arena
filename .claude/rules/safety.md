---
events:
  - "file_write"
  - "file_edit"
---
# Safety Engine, Meta Integrity & Regression Blockers

## 1. Unity Meta File Protection
- **No Missing Meta Files:** You are strictly forbidden from creating, altering, renaming, or deleting any asset source file without executing the exact matching file actions on its corresponding `.meta` file representation. 
- **GUID Preservation:** Modifying asset structural paths must preserve internal asset GUID identifiers exactly. Destroying a `.meta` file or mutating its GUID maps is classified as a severe system break that destroys Inspector scene linkages.

## 2. Multi-Scene Dependency Safeguards
- **Prefab Context Stability:** Before altering root variables inside a base Prefab asset, you must parse the asset database tracking layout to trace what nested variant dependencies or active game scenes utilize this blueprint.
- **Regression Isolation:** You are strictly blocked from editing shared game loops, generic singleton references, or global event channels without displaying a detailed impact table for user validation.

## 3. Pre-Flight Verification Loops
- **The Compilation Metric:** After any file changes, compile verification checks must be run. Code modifications are considered broken if the domain assembly compilation triggers any engine errors, internal script warnings, or invalid structural flags.