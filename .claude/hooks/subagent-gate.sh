#!/usr/bin/env bash
# SubagentStop gate. Fires when a subagent finishes.
# Blocks the implementer from "finishing" if the gate fails, feeding the
# reason back so it continues instead of stopping. Only the implementer is
# gated here; other tiers are allowed to stop freely.
#
# Exit-code semantics (read this before editing):
#   exit 0 + {"decision":"block","reason":...} on stdout  -> block stop, reason fed back
#   exit 0 + no JSON                                        -> allow stop
#   exit 2 + message on stderr                              -> hard block
# A {"decision":"block"} printed and THEN exit 2 is DISCARDED. Keep exit 0 here.
#
set -euo pipefail

input="$(cat)"
agent_type="$(printf '%s' "$input" | grep -o '"agent_type"[[:space:]]*:[[:space:]]*"[^"]*"' | sed -E 's/.*:[[:space:]]*"([^"]*)"/\1/' || true)"

# Only gate the implementer.
if [[ "$agent_type" != "tier-implementer" ]]; then
  exit 0
fi

proj="${CLAUDE_PROJECT_DIR:-.}"
reasons=()

# 1. Secret scan on the working diff (fast, high value).
if git -C "$proj" diff --unified=0 \
     | grep -E -i 'BEGIN (RSA|EC|OPENSSH) PRIVATE KEY|password=|secret=|bao_token|connectionstring=' >/dev/null 2>&1; then
  reasons+=("Possible secret in the diff. Remove it before finishing.")
fi

# 2. (Opt-in) build + test gate for the .NET solution.
#    Uncomment to enforce. Note: this runs on every implementer stop, so it
#    adds real latency; keep it off until the task is close to done, or scope
#    it to a specific project with -p.
# if ! dotnet build -clp:ErrorsOnly "$proj" >/tmp/tier-build.log 2>&1; then
#   reasons+=("Build failing. See /tmp/tier-build.log.")
# fi
# if ! dotnet test "$proj" >/tmp/tier-test.log 2>&1; then
#   reasons+=("Tests failing. See /tmp/tier-test.log.")
# fi

if [[ ${#reasons[@]} -gt 0 ]]; then
  reason="$(printf '%s ' "${reasons[@]}")"
  printf '{"decision":"block","reason":"%s"}\n' "${reason//\"/\\\"}"
  exit 0
fi

exit 0
