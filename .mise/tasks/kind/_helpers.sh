#!/usr/bin/env bash
# Shared helpers for kind test tasks — sourced, not executed directly.

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")"/../../.. && pwd)"
CHART_PATH="${REPO_ROOT}/deploy/helm/wcm-api"
SQL_DIR="${REPO_ROOT}/scripts"
API_IMAGE="wcm-api:0.1.0"

# Detect container runtime
if command -v podman &>/dev/null && podman info &>/dev/null 2>&1; then
  RUNTIME="podman"
else
  RUNTIME="docker"
fi

_info()  { printf '\033[1;34m==> %s\033[0m\n' "$*"; }
_ok()    { printf '\033[1;32m  ✓ %s\033[0m\n' "$*"; }
_warn()  { printf '\033[1;33m  ⚠ %s\033[0m\n' "$*"; }
_err()   { printf '\033[1;31m  ✗ %s\033[0m\n' "$*"; }
