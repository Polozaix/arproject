# HUD resolution and sharpness

## Findings

- The saved active quality index is Low, using Standalone Performant Preset with render scale 1.0 and 4x MSAA. The quality name alone does not indicate the actual XR image resolution.
- The HUD uses world-space geometry and TextMesh Pro SDF text. Its 1000 x 600 RectTransform describes layout units, not a 1000 x 600 bitmap. Increasing those dimensions alone is not a resolution fix.
- The font atlas is 1024 x 1024 with bilinear filtering. There is no evidence yet that replacing it would resolve whole-image pixelation.
- The saved Game view layout has a target size around 871 x 462 and zoom 1.5. This can exaggerate pixelation on the monitor. Saved layout values are not a measurement of the current XR eye buffer.
- User reports pixelation both on the monitor and inside Quest. The precise contribution of eye-buffer resolution, Link compression, display optics and fine UI detail has not been measured.

## Change made

`HUDAnchor` now has `HudRenderQuality`. During PC/Editor execution it clones the active URP asset, sets render scale to 1.25 and MSAA to 4x, and installs the clone as a temporary quality override. This affects scene rendering including the world-space HUD. Unity's installed URP code forwards the render scale to the XR system.

The original pipeline asset is not edited. Disabling the component or leaving Play Mode restores the previous quality override when this component still owns it. The component does not apply the PC override on mobile players, including a standalone Quest build.

Panel rims increased from 1 to 1.5 layout units with a slight contrast increase to make very fine edges more substantial.

## A/B test

1. Reload the updated PersonalARPrototype scene and enter Play Mode.
2. Select HUDAnchor and locate Hud Render Quality. Compare Desktop Render Scale 1.0 and 1.25 while looking at the same text and diagonal edges. For a stronger comparison, try 1.5 only if frame time allows.
3. Scale 1.25 requests 25% more resolution in each dimension, approximately 56% more pixels. Scale 1.5 requests 125% more pixels than 1.0. These are pixel-count calculations, not measured frame-time increases.
4. Use a larger Game view at 1x zoom when inspecting monitor output. For a non-XR desktop check, select a fixed 1920 x 1080 output and view it at 1x if the monitor can fit it. During XR, the Game view may show a headset mirror rather than an independently sized camera output.
5. Compare the monitor mirror and headset while stationary, then turning. If PC output is clean but the headset is blocky, investigate the Link connection, encoding and runtime resolution next. Avoid raising multiple resolution scales at once.
6. Record actual frame time and headset clarity before choosing the demo setting. If movement stutters, return the scale to 1.0 or an intermediate value.

Play Mode Inspector changes are temporary; apply a preferred value outside Play Mode and save the scene. Higher rendering resolution cannot remove physical headset pixel limits or lens blur. If the smallest labels remain difficult, increase their physical text size and simplify content during the graphical pass.

## Validation

All PersonalAR scripts compile against the installed Unity assemblies. Scene component IDs and wiring were checked. Visual improvement and GPU cost remain unverified in Play Mode/headset testing.

Reference: [Unity URP quality settings](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/universalrp-asset.html#quality).
