#!/usr/bin/env bash

set -euo pipefail

tracked_development_settings="$(git ls-files -- ':(glob)**/appsettings.Development.json')"
if [[ -n "$tracked_development_settings" ]]; then
  echo "appsettings.Development.json files must never be tracked:" >&2
  echo "$tracked_development_settings" >&2
  exit 1
fi

if ! git check-ignore --quiet --no-index tools/probe/appsettings.Development.json; then
  echo "The repository must ignore appsettings.Development.json at every depth." >&2
  exit 1
fi

while IFS= read -r settings_file; do
  if grep -q '"ConnectionStrings"[[:space:]]*:' "$settings_file"; then
    echo "ConnectionStrings must not appear in tracked settings: $settings_file" >&2
    exit 1
  fi
done < <(git ls-files -- ':(glob)**/appsettings*.json' ':(exclude,glob)**/*.example.json')

echo "Development settings policy passed."
