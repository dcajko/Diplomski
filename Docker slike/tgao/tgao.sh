#!/bin/sh
echo -ne '\033c\033]0;Diplomski\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/tgao.x86_64" "$@"
