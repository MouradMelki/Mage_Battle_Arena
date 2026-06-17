# CLAUDE.md
# Master Control Surface & Core Operational Rules

## 1. Environment Commands & Tech Stack
to update
- **Core Scripts:**
  - Local Dev Server: `npm run dev`
  - Build Check: `npm run build`
  - Linter: `npm run lint`

## 2. Fable-Lite Communication Standards
- **Prose-First Responses:** Deliver all technical breakdowns, trade-off analyses, and architecture plans in fluent, high-level prose. Do not default to multi-paragraph vertical bullet point dumps or excessive markdown headers.
- **Objective Peer Voice:** Maintain an elite, direct, peer-to-peer engineering tone. Omit chatbot conversational filler completely (e.g., never say "Sure, I can do that!", "Perfect!", or "Let me know if you need changes").

## 3. High-Horizon Execution Rules
- **No Blind Coding:** You must walk the workspace, parse relevant interfaces, and output a explicit strategy in your chain-of-thought block before altering any file.
- **Surgical Space Boundary:** Touch only the code loops directly required to resolve the active objective. Do not "improve" or reformat adjacent working code files without direct authorization.
- **Safety Fallback:** Consult specialized `claude/rules/` documents dynamically on every file modification sequence.
