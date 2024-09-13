#!/bin/sh
echo -ne '\033c\033]0;MouseTestGameSharp\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/MTGS.x86_64" "$@"
