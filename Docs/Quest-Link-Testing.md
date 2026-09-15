# Quest Link live testing

## Setup found on 2026-09-09

- Meta Horizon Link reports Quest 2 connected and active.
- Windows active OpenXR runtime is Meta's `oculus_openxr_64.json`.
- Unity's desktop XR provider was XR Simulation. Changed it to the existing OpenXR loader in `Assets/XR/XRGeneralSettingsPerBuildTarget.asset`. Android settings were already using OpenXR and were left unchanged.
- Run In Background is already enabled.
- The first Play Mode attempt after the correction reached the Oculus runtime but failed at `xrGetSystem` with `XR_ERROR_FORM_FACTOR_UNAVAILABLE`. This is an unavailable headset system, not a C# compilation error. The Link client log showed its previous session had stopped.
- Link reported low headset battery and recommended USB 3. These do not by themselves establish the cause of startup failure.

After the user entered the cable Link session, Play Mode was restarted at approximately 16:13:43. OpenXR successfully progressed through READY, SYNCHRONIZED, VISIBLE, and FOCUSED. Unity created stereo eye textures and its Left Eye mirror displayed the clock and tactical frame. This verifies application/session startup and mirrored rendering. The user's assessment of visibility, readability, and comfort remains pending.

## Build configuration (verified 2026-09-15)

| Setting | Value |
| --- | --- |
| Build scenes | `Assets/Scenes/PersonalARPrototype.unity` (scene 0) then `Assets/Scenes/SampleScene.unity`, both enabled |
| Company / product name | `PersonalAR` / `PersonalAR` |
| Bundle identifier | `com.personalar.prototype` for Android and Standalone |
| Quality level | Low, using Standalone Performant Preset with 4x MSAA |
| XR providers | OpenXR on Android and Standalone; Meta Quest Camera (Passthrough) enabled for both |
| Android SDK | minimum 34, target 34, ARM64 |

Before this pass, Build Settings listed only `SampleScene`, so an Android build would have launched the template scene instead of the HUD, and the identity fields still carried Unity template defaults (`DefaultCompany`, `com.DefaultCompany.MixedRealityTemplate`, `com.unity.template.mr`). Changing the bundle identifier after an app has been installed on a headset or uploaded to a store needs a reinstall or a new store entry, so confirm the final name before distributing builds.

## Live test sequence

1. Connect the Quest and open Meta Horizon Link on the PC.
2. Wear/wake the headset and enter Quest Link inside it. Having the PC app open alone is not the complete test setup.
3. Once the Link dashboard is visible in the headset, start Unity Play Mode in `PersonalARPrototype`.
4. Verify that head movement updates the view and the clock/frame are visible.
5. If XR startup fails before the headset is available, stop Play Mode, restore the Link session, and start Play Mode again.

## Tuning while running

Select `PersonalAR/HUDAnchor` to adjust Distance, Position Smooth Time, and Rotation Sharpness in the Inspector. Select its `HUD` child to adjust the canvas scale or dimensions. Toggle `TacticalFrame (Optional)` to compare the decoration on and off.

Record values you like before stopping Play Mode. Normal scene Inspector changes made during Play Mode are temporary; apply the chosen values again outside Play Mode and save the scene. Code changes can trigger compilation/reloading, so this is a rapid iteration workflow rather than a guarantee of uninterrupted code hot reload.

Initial HUD size (historical baseline): 600 by 300 canvas units at scale 0.001, giving 0.6 by 0.3 metres at distance 1.2 metres. When centred and facing the user this spans approximately 28 by 14 degrees. A 1.5-times scale would span approximately 41 by 21 degrees. These are geometric estimates, not measurements of headset readability or comfort. Game view window size and zoom do not directly describe perceived headset size.

## Evidence

Unity log: `Logs/Editor.log`, initial corrected-provider attempt around 16:09:33 on 2026-09-09.

OpenXR error definition: https://registry.khronos.org/OpenXR/specs/1.1/man/html/xrGetSystem.html

The user subsequently confirmed the headset HUD was visible and requested a larger frame, which produced a 1600 by 900 canvas during that session. **The saved canvas is now 1000 by 600 at scale 0.001**, with distance 1.2 m and follow values of 0.20 s position smooth time and rotation sharpness 8 (re-verified against the scene on 2026-09-15). See HUD-Controls.md for the first feedback record and [Modular-HUD.md](Modular-HUD.md) for the modular shell that replaced the clock-only build.
