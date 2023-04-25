# How to build pkg

## Binary

Copy built output of Visual Studio.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
cp ../../Nano3D/bin/Release/net48/Nano3D.rhp .
```

## Dependencies

Copy other dependencies like Nano3D server executable `printer.exe` next to `Nano3D.rhp`.

## Manifest

Generate `manifest.yml` file by the following command. The `manifest.yml` file is generated only once. Once you have one, keep it with your project and update it for each release.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" spec
```

## build

Build the package file, a file like `nano3d-1.0.0-rh7_13-any.yak` would be generated.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" build
```

# How to publish pkg

## Authentication

Authorize the Yak CLI tool.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" login
```

## Push

Publish pkg.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" push nano3d-1.0.0-rh7_13-any.yak
```

If you just want to test without actually publishing:

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" push --source https://test.yak.rhino3d.com nano3d-1.0.0-rh7_13-any.yak
```

## Check

Check if pkg is pushed fine.

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease Nano3D
```

If you just want to test:

```bash
# Inside /c/Users/m3/source/repos/Nano3D/pkg/dist
"C:\Program Files\Rhino 7\System\Yak.exe" search --source https://test.yak.rhino3d.com --all --prerelease Nano3D
```

