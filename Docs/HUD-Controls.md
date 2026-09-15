# Clock motion and optional tactical frame

> **Status: historical record, reconciled against the scene on 2026-09-15.** This note describes the first clock-only HUD pass. That runtime has since been replaced by the modular shell; see [Modular-HUD.md](Modular-HUD.md) for current behaviour. Several numbers in the original text described a scene state that no longer exists (0.07 s smooth time, rotation sharpness 16, a 1600 by 900 canvas, and a `ClockWidget`-driven clock). The verified values for the scene as committed today are the table below; the superseded description is kept further down as history.

## Current saved state (verified against the scene, 2026-09-15)

| Item | Saved value | Location |
| --- | --- | --- |
| Shell canvas | 1000 by 600 layout units, local scale 0.001 | `PersonalAR/HUDAnchor/HUD` RectTransform |
| Follow | distance 1.2 m, vertical offset 0, position smooth time 0.20 s, rotation sharpness 8, maximum angular lag 12 degrees | `PersonalAR/HUDAnchor` — `SoftHeadFollow` |
| Clock | Built at runtime as the `clock` module in `HudSlot.TopRight` by `ModularHud`, combining time and date | `Assets/PersonalAR/Scripts/UI/ModularHud.cs` |
| Scene `Clock` object | Retained only as the TextMesh Pro font source. `ModularHud.Start` disables the GameObject, so `ClockWidget` never updates it | `PersonalAR/HUDAnchor/HUD/Clock` |
| Decoration | `TacticalFrame (Optional)`: line width 4, inset 8, colour (0.25, 0.90, 1.00, alpha 0.65) | `PersonalAR/HUDAnchor/HUD/TacticalFrame (Optional)` |
| Quality | Active level Low, using Standalone Performant Preset. `HudRenderQuality` applies render scale 1.25 with 4x MSAA on PC/Quest Link only | `ProjectSettings/QualitySettings.asset`, `HUDAnchor/HudRenderQuality` |
| Build scene | `Assets/Scenes/PersonalARPrototype.unity` is Build Settings scene 0 | `ProjectSettings/EditorBuildSettings.asset` |

The tuning notes in this file apply to `Assets/Scenes/PersonalARPrototype.unity`.

## Motion

The hierarchy is `PersonalAR/HUDAnchor/HUD` (Canvas) with `Clock` and `TacticalFrame (Optional)` children. `HUDAnchor` is outside the camera hierarchy and its `SoftHeadFollow` component targets Main Camera. `ModularHud` and `SpatialObjectives` are components of the `HUD` object itself and build their views at runtime, so most of the HUD is not authored in the scene.

Current follow settings are distance 1.2 m, position smooth time 0.20 s, rotation sharpness 8 and maximum angular lag 12 degrees. These are starting choices awaiting headset feedback, not measured comfort results. Increase position smooth time for more positional lag; increase rotation sharpness for faster rotational following. The first update places the HUD immediately so it does not fly in from the scene origin. Re-enabling the component resets this placement. These values were retuned from the earlier 0.07 / 16 pair recorded below; see [Modular-HUD.md](Modular-HUD.md) for the change and its stated reasoning.

The scene's `Clock` object still carries the legacy top-right anchors and pivot, a 32-unit right margin, a 28-unit top margin and a 36-point font from the earlier 1600 by 900 canvas. That object is disabled at runtime, so those anchors are inert: the visible time and date come from the `clock` module, which `ModularHud` lays out in `HudSlot.TopRight` with its own heading and dismissal button. The decorative frame remains independent of the clock.

## Decoration

Select `PersonalAR/HUDAnchor/HUD/TacticalFrame (Optional)`.

- Uncheck the GameObject to hide the whole frame, or delete this child to remove it.
- Change the Graphic Color (including alpha) to adjust tint and opacity.
- Change Line Width and Inset on `TacticalHudFrame` to adjust the linework.
- To add it to another canvas, create a UI child with a RectTransform, stretch it to the parent with zero offsets, then add `TacticalHudFrame` and choose a colour.

The frame uses a small generated UI mesh rather than texture assets or additional packages. It stretches with the canvas and does not receive raycasts. The marks are decoration, not live battery, heading, tracking, or weapon information. No idle animation is added.

## Verification and next headset check (recorded at that stage)

The three custom scripts that existed at that stage compiled using Unity's bundled C# compiler and the project's assembly references. The project now has eleven PersonalAR scripts; see [Modular-HUD.md](Modular-HUD.md). Scene structure checks verify unique object IDs, the follow target, parent/child references, and the optional frame wiring. This does not replace an Editor import, Play Mode, or Quest Link test.

Unity was already open during the file edits. If prompted about an externally changed scene, reload the changed scene. Preserve any unrelated unsaved Editor work in a separate scene before reloading; do not overwrite the updated scene with a stale in-memory copy.

Next in Quest Link (this checklist describes the clock-only build):

1. Confirm the scene imports without errors, then enter Play Mode.
2. Verify the clock starts at the top-right and follows slow head movements.
3. Compare frame enabled/disabled for readability and distraction.
4. Check initial placement and movement after disabling/re-enabling the follow component.
5. Record the actual runtime settings and observations using `Testing-and-Observations.md`.

Live headset visibility and initial comfort feedback are now recorded in [HUD-001](TestSessions/2026-09-09-hud-size-exploration.md). Passthrough has not been verified.

## First headset feedback (historical)

On 2026-09-09 the developer confirmed that the HUD was visible in Quest Link and described it as great, but wanted the frame closer to the edges of the view. Expanded the canvas from 600 by 300 to 1600 by 900, keeping scale 0.001, distance 1.2 m, text size, and smoothing unchanged. This gives an approximate centred angular extent of 67 by 41 degrees. The developer subsequently reported uncomfortable corner viewing while still liking the borders, then preferred 1000 by 600 after manual tuning. See [HUD-001](TestSessions/2026-09-09-hud-size-exploration.md) for evidence, the reported-width discrepancy, and saving status.

> **Reconciliation note (2026-09-15):** the canvas was subsequently saved at 1000 by 600 layout units and the follow values were retuned to 0.20 s position smooth time with rotation sharpness 8. The measurements in the paragraph above describe 2026-09-09 only and should not be read as the current configuration. The clock is now drawn by the `clock` module rather than by `ClockWidget`, which survives only as the font source.
