# Clock motion and optional tactical frame

Open `Assets/Scenes/PersonalARPrototype.unity`.

## Motion

The hierarchy is now `PersonalAR/HUDAnchor/HUD/Clock`. `HUDAnchor` is outside the camera hierarchy and its `SoftHeadFollow` component targets Main Camera.

Starting scene settings are distance 1.2 m, position smooth time 0.07 s, and rotation sharpness 16. These are starting choices awaiting headset feedback, not measured comfort results. Increase position smooth time for more positional lag; increase rotation sharpness for faster rotational following. The first update places the HUD immediately so it does not fly in from the scene origin. Re-enabling the component resets this placement.

The clock uses top-right anchors and pivot, a 32-unit right margin and 28-unit top margin in the 1600 by 900 canvas. It remains independent of the decorative frame.

## Decoration

Select `PersonalAR/HUDAnchor/HUD/TacticalFrame (Optional)`.

- Uncheck the GameObject to hide the whole frame, or delete this child to remove it.
- Change the Graphic Color (including alpha) to adjust tint and opacity.
- Change Line Width and Inset on `TacticalHudFrame` to adjust the linework.
- To add it to another canvas, create a UI child with a RectTransform, stretch it to the parent with zero offsets, then add `TacticalHudFrame` and choose a colour.

The frame uses a small generated UI mesh rather than texture assets or additional packages. It stretches with the canvas and does not receive raycasts. The marks are decoration, not live battery, heading, tracking, or weapon information. No idle animation is added.

## Verification and next headset check

The three custom scripts compiled using Unity's bundled C# compiler and the project's assembly references. Scene structure checks verify unique object IDs, the follow target, parent/child references, and the optional frame wiring. This does not replace an Editor import, Play Mode, or Quest Link test.

Unity was already open during the file edits. If prompted about an externally changed scene, reload the changed scene. Preserve any unrelated unsaved Editor work in a separate scene before reloading; do not overwrite the updated scene with a stale in-memory copy.

Next in Quest Link:

1. Confirm the scene imports without errors, then enter Play Mode.
2. Verify the clock starts at the top-right and follows slow head movements.
3. Compare frame enabled/disabled for readability and distraction.
4. Check initial placement and movement after disabling/re-enabling the follow component.
5. Record the actual runtime settings and observations using `Testing-and-Observations.md`.

Live headset visibility and initial comfort feedback are now recorded in [HUD-001](TestSessions/2026-09-09-hud-size-exploration.md). Passthrough has not been verified.

## First headset feedback

On 2026-09-09 the developer confirmed that the HUD was visible in Quest Link and described it as great, but wanted the frame closer to the edges of the view. Expanded the canvas from 600 by 300 to 1600 by 900, keeping scale 0.001, distance 1.2 m, text size, and smoothing unchanged. This gives an approximate centred angular extent of 67 by 41 degrees. The developer subsequently reported uncomfortable corner viewing while still liking the borders, then preferred 1000 by 600 after manual tuning. See [HUD-001](TestSessions/2026-09-09-hud-size-exploration.md) for evidence, the reported-width discrepancy, and saving status.
