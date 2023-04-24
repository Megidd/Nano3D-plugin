# How to build pkg

## Built

Copy built output of Visual Studio.

```bash
cp ../Nano3D/bin/Release/net48/Nano3D.rhp dist/
```

## Dependencies

Copy other dependencies if any.

## Manifest

Generate `manifest.yml` file by the following command.

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" spec
```

## build

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" build
```

