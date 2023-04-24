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
"C:\Program Files\Rhino 7\System\Yak.exe" spec
```

## build

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" build
```

