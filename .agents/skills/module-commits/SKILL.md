---
name: module-commits
description: "Trigger: commit, crear commit, guardar cambios, conventional commits. Enforces conventional commit standards organized by module/scope and subject without AI attribution."
license: Apache-2.0
metadata:
  author: j053_
  version: "1.0"
---

## Activation Contract

Activate this skill when creating or planning Git commits to ensure changes are staged atomically by module and formatted according to Conventional Commits.

## Hard Rules

- **Format Constraint**: All commit messages MUST follow the format `<type>(<scope>): <subject>`.
  - Allowed types: `feat`, `fix`, `refactor`, `docs`, `style`, `test`, `chore`, `perf`.
  - Scope: Must represent the specific game module or system (e.g., `building`, `hazards`, `grid`, `ui`, `audio`).
  - Subject: Concise, imperative description in lowercase, without a trailing period.
- **Atomic Module Commits**: Group modified files into separate commits by module. Never combine unrelated modules into a single commit.
- **NO AI Attribution**: Never include `Co-Authored-By`, `Generated-by`, or any AI tool signatures in commit messages.

## Execution Steps

1. **Analyze Staged/Unstaged Changes**: Inspect `git status` and `git diff` to identify modified files.
2. **Group by Module**: Group changed files by their functional system (e.g., `GridBuildController.cs` under `building`, `HazardsController.cs` under `hazards`).
3. **Draft Messages**: Format each commit header as `<type>(<scope>): <subject>`.
4. **Execute Atomic Commits**: Stage files per module (`git add <files>`) and execute `git commit -m "<type>(<scope>): <subject>"` for each group.
