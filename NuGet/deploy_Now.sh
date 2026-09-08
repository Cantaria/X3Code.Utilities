#!/usr/bin/env bash
#
# Publiziert die gebauten NuGet-Pakete aus diesem Ordner.
#
# Frueher fragte das Skript EINE Version ab und setzte sie fuer alle Pakete ein.
# Da die Pakete unterschiedliche Versionen haben, wurde damit faktisch nur ein
# einziges Paket gepusht - der Rest meldete "does not exist" und der Release sah
# trotzdem erfolgreich aus. Jetzt wird gepusht, was tatsaechlich da ist.

set -euo pipefail

PACKAGE_SOURCE=github
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Pakete, die bewusst NICHT veroeffentlicht werden.
# X3Code.Infrastructure.Mobile wird ausserhalb dieses Repos gepflegt und laeuft nicht
# ueber dieses Deploy. Das Projekt baut weiterhin ein Paket, es wird hier nur nicht gepusht.
EXCLUDED_PACKAGE_IDS=(
    "X3Code.Infrastructure.Mobile"
)

is_excluded() {
    local file_name="$1"
    for excluded in "${EXCLUDED_PACKAGE_IDS[@]}"; do
        [[ "$file_name" == "$excluded".[0-9]*.nupkg ]] && return 0
    done
    return 1
}

cd "$SCRIPT_DIR"

shopt -s nullglob
all_packages=( *.nupkg )
shopt -u nullglob

if [ ${#all_packages[@]} -eq 0 ]; then
    echo "Keine .nupkg in $SCRIPT_DIR gefunden. Zuerst 'dotnet build X3Code.Utilities.sln' ausfuehren."
    exit 1
fi

packages=()
for package in "${all_packages[@]}"; do
    if is_excluded "$package"; then
        echo "Uebersprungen (ausgeschlossen): $package"
    else
        packages+=( "$package" )
    fi
done

if [ ${#packages[@]} -eq 0 ]; then
    echo "Nach dem Ausschluss bleibt nichts zu pushen."
    exit 1
fi

echo
echo "Folgende Pakete werden nach '$PACKAGE_SOURCE' gepusht:"
for package in "${packages[@]}"; do
    echo "  - $package"
done

echo
read -r -p "Fortfahren? [j/N] " confirm
case "$confirm" in
    j|J|y|Y) ;;
    *) echo "Abgebrochen."; exit 0 ;;
esac

# -s: die Eingabe wird nicht im Terminal angezeigt
read -r -s -p "API-Key: " apikey
echo

if [ -z "$apikey" ]; then
    echo "Kein API-Key angegeben. Abbruch."
    exit 1
fi

failed=()
for package in "${packages[@]}"; do
    echo
    echo "==> $package"
    # --no-symbols: GitHub Packages nimmt keine .snupkg entgegen.
    # --skip-duplicate: eine bereits veroeffentlichte Version ist kein Fehler.
    if dotnet nuget push "$package" \
        --source "$PACKAGE_SOURCE" \
        --api-key "$apikey" \
        --no-symbols \
        --skip-duplicate; then
        echo "    ok"
    else
        echo "    FEHLGESCHLAGEN"
        failed+=( "$package" )
    fi
done

echo
if [ ${#failed[@]} -eq 0 ]; then
    echo "Alle ${#packages[@]} Pakete verarbeitet."
else
    echo "${#failed[@]} von ${#packages[@]} Paketen fehlgeschlagen:"
    for package in "${failed[@]}"; do
        echo "  - $package"
    done
    exit 1
fi
