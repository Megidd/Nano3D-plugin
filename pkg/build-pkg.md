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

# How to publish pkg

## Authentication

Authorize the Yak CLI tool.

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" login
```

## Push

Publish pkg.

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" push marmoset-1.0.0-rh6_18-any.yak
```

If you just want to test without actually publishing:

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" push --source https://test.yak.rhino3d.com marmoset-1.0.0-rh6_18-any.yak
```

## Check

Check if pkg is pushed fine.

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease marmoset
```

If you just want to test:

```bash
"C:\Program Files\Rhino 7\System\Yak.exe" search --source https://test.yak.rhino3d.com --all --prerelease marmoset
```

