#!/bin/sh

### Run this script by Git bash.

cp ../Nano3D/bin/Debug/net48/Nano3D.rhp dist/

### Run only once to generate `manifest.yml` file:
"C:\Program Files\Rhino 7\System\Yak.exe" spec

"C:\Program Files\Rhino 7\System\Yak.exe" build

