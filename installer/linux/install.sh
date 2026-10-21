#!/bin/sh
set -eu

source_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
data_dir=${XDG_DATA_HOME:-"$HOME/.local/share"}
install_dir="$data_dir/transring/application"
bin_dir="$HOME/.local/bin"
desktop_file="$data_dir/applications/com.transring.app.desktop"
if [ -e "$install_dir" ] || [ -L "$install_dir" ]; then
    printf '%s\n' "An installation already exists at $install_dir. Run its uninstall.sh first."
    exit 1
fi
if [ -e "$bin_dir/transring" ] || [ -L "$bin_dir/transring" ]; then
    printf '%s\n' "$bin_dir/transring already exists. Remove or rename it before installing TransRing."
    exit 1
fi
if [ -e "$desktop_file" ] || [ -L "$desktop_file" ]; then
    printf '%s\n' "$desktop_file already exists. Remove or rename it before installing TransRing."
    exit 1
fi
command -v python3 >/dev/null
test -x "$source_dir/TransRing"
test -x "$source_dir/transring-cli"
mkdir -p "$data_dir/transring" "$bin_dir" "$data_dir/applications"
cp -R "$source_dir" "$install_dir"
printf '%s\n' 'com.transring.app' > "$install_dir/.transring-package"
ln -s "$install_dir/transring-cli" "$bin_dir/transring"
python3 - "$install_dir" "$desktop_file" <<'PY'
import pathlib
import sys

app, desktop = map(pathlib.Path, sys.argv[1:])
def quoted_exec(value):
    value = value.replace('%', '%%')
    for character in ('\\', '"', '`', '$'):
        value = value.replace(character, '\\' + character)
    return '"' + value.replace('\\', '\\\\').replace('\n', '\\n').replace('\r', '\\r') + '"'

def desktop_value(value):
    return str(value).replace('\\', '\\\\').replace('\n', '\\n').replace('\r', '\\r')

desktop.write_text(
    '[Desktop Entry]\nType=Application\nName=TransRing\n'
    'Comment=Convert files with TransRing\n'
    f'Exec={quoted_exec(str(app / "TransRing"))} --convert %F\n'
    f'Icon={desktop_value(app / "transring.png")}\n'
    'Terminal=false\nCategories=Utility;\nX-TransRing-Managed=true\n', encoding='utf-8')
PY
if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database "$data_dir/applications"
fi
printf '%s\n' "Installed TransRing at $install_dir" "CLI: $bin_dir/transring (add $bin_dir to PATH if needed)."
