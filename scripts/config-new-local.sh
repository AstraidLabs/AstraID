#!/usr/bin/env bash
set -e
TEMPLATE="src/AstraID.Api/appsettings.Local.json.example"
TARGET="src/AstraID.Api/appsettings.Local.json"
if [ -f "$TARGET" ]; then
  echo "$TARGET already exists" >&2
  exit 1
fi
cp "$TEMPLATE" "$TARGET"
echo "Created $TARGET. Replace placeholder values (e.g., CHANGE_ME) before running the app."
