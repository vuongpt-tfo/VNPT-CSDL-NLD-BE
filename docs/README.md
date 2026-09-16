# Documentation Map — VNPT-CSDL-NLD-BE

> How this repo's knowledge is organized: one folder per kind of document, so a new doc has one obvious home and an assistant/person can discover what's relevant by scanning summaries rather than a hardcoded list. Last reviewed: 2026-09-16.

Backend của module báo cáo/dashboard VNPT SIMB_CSDL Người lao động. Xem `decision-log.md` cho các quyết định kiến trúc.

## Folder map

| Folder | Holds | A doc belongs here when… |
|---|---|---|
| `docs/project/` | Orientation, the big picture | it answers "what is this, who is it for, what are the standing commitments." |
| `docs/architecture/` | System-wide rules & invariants | it states a rule that cuts across the whole backend, not one module (e.g. clean-architecture layering: API → Application → Domain → Infrastructure). |
| `docs/modules/` | Per-component ownership docs | it documents what one part owns (e.g. `dashboard-d01.md`, `labour-identity.md`, `labour-insurance.md`). |
| `docs/standards/` | Conventions & foot-guns | it tells a contributor *how* to build here — coding conventions, EF Core patterns, etc. |
| `docs/recipes/` | Short checklist how-tos | a fast-path, task-scoped checklist that links to a source-of-truth doc rather than restating it. |
| `docs/decisions/` | Decision log, risks, open questions | it records a choice that was made (or is pending), why, and what it supersedes. **Authoritative — most recent entry beats any older statement elsewhere.** |
| `docs/operations/` | Runtime/operational knowledge | deployment, environments, runbooks, incidents, audits, migrations — filename suffixed `-YYYY-MM-DD`. |
| `docs/archive/` | Superseded documents | it's been replaced and the replacement is recorded in `decisions/`. Kept for history, never cited as current. |
| `reviews/` (repo root) | Independent review reports | an adversarial review of a completed phase before proceeding to the next one. |

## Conventions (every doc)

1. One `# H1` title.
2. A one-line `>` blockquote summary right under the title, kept accurate — this is what gets scanned before anyone opens the file.
3. A `Last reviewed: YYYY-MM-DD` marker (dated operations/review docs use "Report/Review date:" instead).
4. Reference other docs by bare filename, optionally with a section anchor (`decision-log.md`, `architecture.md §2`) — never a relative path.
5. Naming: lowercase `kebab-case.md`, named by subject — except dated operational reports (`-YYYY-MM-DD` suffix).

## Rules

- Log in the same change as the code, never as a follow-up.
- Any AI-assistant bootstrap file (`CLAUDE.md`, etc.) only points into this structure — it never restates a rule that belongs in a doc here.
- If two docs conflict and nothing in `decisions/` settles it: stop and ask, don't guess.
