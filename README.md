# Godot Volumetric Rendering

Volumetric rendering implemented in Godot 4.4. The example project can be
played on desktop and Meta Quest 3. The shader used for rendering can
be used in any other Godot project as well.

## Setup Guide
In order to use the application, prepare the following things:
* Make sure to have some data ready.
  * For now, the images need to be provided as multiple `.png` files, with each of those representing one slice. (All need to be in the same resolution and in `Rbg8` format)
  * For splitting, the tool `tiffsplit` is provided in the `/scripts` directory of this repo
  * The images have to be named `slice_000.png`, ..., `slince_<n>.png` (The tool does that for you)
  * Place images that belong to the same volume into `/assets/<volume>/`
* Once the images are prepared and imported by Godot, select `VolumetricMesh` in the main scene and fill in the appropriate parameters in the object inspector:
  * X/Y Resolution: The respective resolution value of the images
  * Depth Resolution: The depth of the volume, i.e. how many slices there are
  * Slice Directory: The name of the directory the slides are located in (`<volume>` from the above path)
  * Sampling Steps: How many steps are taken through the volume; more steps lead to higher precision at the cost of performance
* If you want to deploy on Meta Quest 3:
  * Make sure have the Android Build Template installed and the Android SDK and JDK paths configured correctly, see [Godot Docs](https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html) for details.
  * Enable developer mode on your device (see [documentation](https://developers.meta.com/horizon/documentation/native/android/mobile-device-setup/))

## Controls
Todo: write about keyboard inputs here

## Benchmarks
All Benchmarks listed here are done with the `1170_gold_img` data set
and the shader's default settings. Godot has Vsync turned off. Viewport dimensions are 1152x648.

### MacBook Air M3
Program is running with power supply (not on battery).

|                | Value              |
|:---------------|:-------------------|
| Frames Counted | 3835               |
| Average:       | 253.77919293430318 |
| Minimum:       | 112.60415884693342 |
| Maximum:       | 396.63938268494314 |
| Deviation:     | 47.985137581993136 |

### RX Vega 56

### RX Vega 56 - VR

### Meta Quest 3 (Standalone)
Left hand tracker must be set to `/user/fbhandaim/left` in order to get Godot to listen to the left hand input.
Note, that this will mess up the rotation of the hand models.

|                | Value              |
|:---------------|:-------------------|
| Frames Counted | 363                |
| Average:       | 25.034885077865844 |
| Minimum:       | 10.296399006054282 |
| Maximum:       | 49.80823828261193  |
| Deviation:     | 7.3812614800931495 |