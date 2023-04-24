# How to build pkg

## Built

Copy built output of Visual Studio.

```bash
cp ../Nano3D/bin/Release/net48/Nano3D.rhp dist/
```

## Dependencies

Copy other dependencies if any.

## Manifest

Generate `manifest.yml` file by the following command. The `manifest.yml` file is generated only once. Once you have one, keep it with your project and update it for each release.

```bash
cd dist/
"C:\Program Files\Rhino 7\System\Yak.exe" spec
```

## build

Build the package file, a file like `nano3d-1.0.0-rh7_13-any.yak` would be generated.

```bash
cd dist/
"C:\Program Files\Rhino 7\System\Yak.exe" build
```

