# CLAUDE.md
# Master Control Surface & Core Unity Engine Execution Rules

## 1. Environment Commands & Tech Stack
- **Core Tech Stack:** Unity Engine, C# (Modern LTS), Assembly Definitions (`.asmdef`).
- **Target Environments:** Cross-Platform Client Architecture (Standalone PC / Console Systems).
- **Core Verification Scripts & CLI Actions:**
  - Execute EditMode Tests: `<UnityPath> -batchmode -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml`
  - Execute PlayMode Tests: `<UnityPath> -batchmode -runTests -testPlatform PlayMode -testResults Logs/playmode-results.xml`
  - Silent Command-Line Build: `<UnityPath> -batchmode -quit -projectPath . -executeMethod BuildPipelineScript.PerformBuild`

## 2. Interactive Presentation & Formatting Rules (CRITICAL)
You are strictly forbidden from outputting flat walls of prose. Every codebase sweep, script audit, or architectural response must mirror this exact visual blueprint:

- **Colored Status Circles (The Triage Dots):** Use colored circles to categorize scripts, scene status, and asset configuration states at a glance:
  - `🔴` = Hardcoded references / GC allocation vulnerabilities / Script memory leaks worth refactoring immediately.
  - `🟡` = Partial implementation / Missing script execution order dependencies / Monobehaviour components lacking caching optimization.
  - `🟢` = Production ready / Clean decoupled data structures / Modular ScriptableObject states.
- **The Prioritized Action Table:** When planning an execution path or mapping next steps, visualize the work in a clean Markdown table ranked strictly by **Priority & Impact** (from highest performance/stability risk to lowest):
  | Priority | Task / What to do | Rationale / Impact | Status / Risk |
  | :---: | :--- | :--- | :--- |
- **The Pragmatic Audit Table:** When performing codebase sweeps or analyzing structural code issues, split the findings into an explicit markdown table:
  | # | Where | Discrepancy Value | Code / Context Note |
- **Point-by-Point Interactive Headers:** Group your responses into explicit numbered execution milestones using semantic, bolded emoji-anchored headers:
  - `1. ✅ [Script Action Taken / Class Modified]`
  - `2. 📋 [C# Architecture Injection / Implementation Specifics]`
  - `3. ❓ [Unity Lifecycle Challenge or Lifecycle Question?] — [Direct, Immediate Answer]`
  - `4. 🔒 [Data Isolation Boundary / Anti-Cheat Status] — [Clear Reassurance]`
- **Dev Workflow Mapping:** Display raw terminal workflows or Unity Editor build steps as sequential, inline execution trails (e.g., `Modify Codebase → Recompile Domain → Validation via EditMode Tests → Verify Meta File States`).
- **Tone:** Professional, direct, and collaborative. Skip conversational filler.

## 3. High-Horizon Execution Rules
- **No Blind Coding:** Walk the workspace dependencies, parse relevant script interactions, and output your execution strategy inside an internal `<thinking_scratchpad>` block before altering any C# script.
- **Surgical Space Boundary:** Touch only lines explicitly required to resolve the active objective. Do not reformat or "optimize" adjacent code layouts or asset components without permission.
- **Policy Routing:** Consult specific `.claude/rules/` guidelines dynamically on every script modification sequence.