#!/bin/zsh

# Array of mappings: class -> shortcut
typeset -A replacements
replacements=(
  "AutoGameStats" "G.GS"
  "AutoGameFlow" "G.GF"
  "AutoShipStats" "G.SS"
  "AutoWeaponDatabase" "G.WD"
  "AutoBackground" "G.BG"
)

# Loop through all .cs files outside the autoload folder
find . -type f -name "*.cs" -not -path "./autoload/*" | while read -r file; do
  for class in "${(@k)replacements}"; do
    shortcut="${replacements[$class]}"
    sed -i '' "s/${class}\.Instance/${shortcut}/g" "$file"
  done
done

echo "Replacement complete."
