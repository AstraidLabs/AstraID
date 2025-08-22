#!/usr/bin/env bash
# Examples:
#   export AstraId__Issuer=https://id.example.com
#   export ConnectionStrings__Default="Server=..."
#   ./scripts/config-set.sh copy
#   ./scripts/config-set.sh validate

set -e
cmd=$1
case "$cmd" in
  copy)
    cp -n src/AstraID.Api/appsettings.Local.example.json src/AstraID.Api/appsettings.Local.json
    echo "Local config created at src/AstraID.Api/appsettings.Local.json"
    ;;
  validate)
    curl -s http://localhost:5000/_diag/config/validate
    ;;
  *)
    echo "Usage: config-set.sh {copy|validate}"
    ;;
 esac
