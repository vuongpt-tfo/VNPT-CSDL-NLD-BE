#!/usr/bin/env bash
# Installs repo git hooks into .git/hooks (not versioned by git itself).
set -euo pipefail
repo_root="$(git rev-parse --show-toplevel)"
cp "$repo_root/scripts/git-hooks/pre-commit" "$repo_root/.git/hooks/pre-commit"
chmod +x "$repo_root/.git/hooks/pre-commit"
echo "Installed pre-commit hook."
