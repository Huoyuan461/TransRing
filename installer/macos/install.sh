#!/bin/sh
set -eu

source_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
install_dir="$HOME/Applications/TransRing.app"
bin_dir="$HOME/.local/bin"
if [ -e "$install_dir" ] || [ -L "$install_dir" ]; then
    printf '%s\n' "An installation already exists at $install_dir. Run uninstall.sh first."
    exit 1
fi
if [ -e "$bin_dir/transring" ] || [ -L "$bin_dir/transring" ]; then
    printf '%s\n' "$bin_dir/transring already exists. Remove or rename it before installing TransRing."
    exit 1
fi
test -x "$source_dir/TransRing.app/Contents/MacOS/TransRing"
mkdir -p "$HOME/Applications" "$bin_dir"
/usr/bin/ditto "$source_dir/TransRing.app" "$install_dir"
ln -s "$install_dir/Contents/MacOS/transring-cli" "$bin_dir/transring"
printf '%s\n' "Installed $install_dir" "CLI: $bin_dir/transring (add $bin_dir to PATH if needed)."
printf '%s\n' 'Open TransRing from Applications. Grant Accessibility and Finder Automation access when requested.'
