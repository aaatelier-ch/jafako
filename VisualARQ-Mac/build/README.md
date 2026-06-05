# Build output

This folder is where the compiled plugin package (`VisualARQMac.rhp`) is
expected to live. Binaries are **not** committed to the repository (see the
root `.gitignore`).

## Producing the .rhp

A Rhino plugin is just a renamed managed assembly. The project file sets
`<TargetExt>.rhp</TargetExt>`, so a Release build already emits
`VisualARQMac.rhp`:

```bash
cd ..
dotnet build src/VisualARQMac/VisualARQMac.csproj -c Release
cp src/VisualARQMac/bin/Release/net7.0/VisualARQMac.rhp build/
```

## macOS bundle (optional)

For distribution you can wrap the assembly in a macOS bundle layout:

```bash
cd build
mkdir -p VisualARQMac.rhp/Contents
cp ../src/VisualARQMac/bin/Release/net7.0/VisualARQMac.dll VisualARQMac.rhp/Contents/

cat > VisualARQMac.rhp/Contents/Info.plist <<'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>VisualARQ-Mac</string>
    <key>CFBundleIdentifier</key>
    <string>com.visualarq-mac.rhino-plugin</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>NSPrincipalClass</key>
    <string></string>
</dict>
</plist>
EOF
```

Then copy `VisualARQMac.rhp` into
`~/Library/Application Support/McNeel/Rhinoceros/MacPlugIns/` and restart Rhino.
