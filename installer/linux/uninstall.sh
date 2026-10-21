#!/bin/sh
set -eu

data_dir=${XDG_DATA_HOME:-"$HOME/.local/share"}
install_dir="$data_dir/transring/application"
desktop_file="$data_dir/applications/com.transring.app.desktop"
python3 - "$data_dir" "$install_dir" <<'PY'
import pathlib
import sys

root, target = map(pathlib.Path, sys.argv[1:])
expected = root.resolve() / 'transring' / 'application'
if target.is_symlink() or target.resolve() != expected:
    raise SystemExit('Refusing to remove an installation outside its expected directory.')
if (target / '.transring-package').read_text(encoding='utf-8').strip() != 'com.transring.app':
    raise SystemExit('The directory is not an installation created by TransRing.')
PY
"$install_dir/TransRing" --uninstall-cleanup
if [ -L "$HOME/.local/bin/transring" ] && [ "$(readlink "$HOME/.local/bin/transring")" = "$install_dir/transring-cli" ]; then
    rm -- "$HOME/.local/bin/transring"
fi
if [ -f "$desktop_file" ] && grep -q '^X-TransRing-Managed=true$' "$desktop_file"; then
    rm -- "$desktop_file"
fi
rm -rf -- "$install_dir"
printf '%s\n' 'TransRing was removed. Your settings and conversion results were kept.'
