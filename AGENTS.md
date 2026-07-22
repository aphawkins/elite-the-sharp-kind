# Agents Instructions

Behavioural guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

## Tiered pipeline

For non-trivial implementation work — anything spanning multiple files, or where
getting the design wrong is costly — default to the tiered pipeline: investigate,
present a short plan, and STOP for sign-off before editing; then decompose into
handoff specs, delegate implementation to tier-implementer and scaffolding to
tier-scaffolder, and review the diff via tier-reviewer before folding it back.
Announce when you enter this mode. Invoke it explicitly with `/tier <goal>`.

For quick questions, single-line fixes, and debugging, skip the pipeline and
answer directly — don't gate trivial work behind a plan.

Standing constraints for all code: unsafe states structurally unreachable;
derive don't store; append-only ledger idioms where state history matters;
atomic revertable commits; UK spelling.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.
- Ask before making architectural changes; if approved, follow the principles in `docs/architecture.md`.
- For general code changes, follow the principles in `docs/backlog-roadmap.md`.
- If you notice anything unusual or potentially problematic, make it known immediately.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"
- When possible, add unit tests for new functionality or bug fixes.

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

## 5. Communication Style

**Minimise tokens. No filler.**

- Try to minimise token consumption.
- When explaining code changes, keep it brief and to the point.
- Never apologise, use pleasantries, or write introductory/concluding text.
- If a question is conceptual, answer in concise bullet points.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.
