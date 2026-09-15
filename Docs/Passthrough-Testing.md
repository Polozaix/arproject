# Test the HUD over the real room

## Project configuration

The installed Unity OpenXR Meta 2.5.1 package supports passthrough through AR Foundation's camera subsystem. Meta Quest Camera (Passthrough) is already enabled for Standalone and Android. The scene now explicitly overrides its main camera to Solid Color with background alpha 0 and keeps AR Camera Manager enabled. The current performant URP preset has HDR off, camera post-processing is off, and the renderer uses Intermediate Texture Auto.

No camera-image capture is needed to display passthrough. The Meta runtime composites it behind the Unity image. We have not verified a live passthrough session for this revision.

## Quest Link setup

1. Save Unity work before restarting the Editor.
2. In the Meta Horizon Link PC app, open **Settings > Beta**.
3. Enable **Developer Runtime Features** and **Passthrough over Meta Quest Link**. These are developer features; if unavailable, check developer-account/device setup against Meta's instructions.
4. Restart Unity after enabling Link features. Reconnect the headset and enter Quest Link.
5. Open the updated PersonalARPrototype scene and enter Play Mode.
6. Inspect inside the headset: the room should appear behind the HUD. This uses app passthrough; opening the system passthrough view alone is not equivalent.

[Meta: Passthrough over Link](https://developers.meta.com/horizon/documentation/unity/unity-passthrough-use-over-link/)

[Meta: Link development setup](https://developers.meta.com/horizon/documentation/unity/unity-link/)

## Readable transparent panels

Open **Modules > CONTROLS > Settings > AR glass mode**.

- Ordinary module backgrounds: 40% opacity.
- Central focus background: 60% opacity.
- Detached objective card backgrounds: 65% opacity.
- Library background: 80% opacity to support editing.
- Button backgrounds, including X, Modules, Undo, library controls and objective-card controls, use 40% opacity in glass mode. Newly created buttons inherit the mode. Text alpha is unchanged. No whole-HUD CanvasGroup fade is used.

Each module's Settings also offers Glass (35%), Light (55%), Balanced (82%) and Solid (100%). These are starting values to test, not guaranteed readability thresholds. Switching AR glass mode off restores the module background values captured when it was enabled. Settings remain session-only. AR glass mode changes UI styling; it does not toggle the platform passthrough service.

Compare against a white wall, a dark wall and a cluttered part of the room. Raise opacity on a specific module if the background interferes with its text. Prefer larger text and stronger local backing over fading the entire HUD.

## If the room stays black

- Confirm both PC Link developer toggles are enabled, and restart Unity after changing them.
- Confirm Meta is the active OpenXR runtime, and the headset is in an active Link session.
- Check the Unity Console for passthrough extension or AR camera subsystem failures.
- Confirm the camera is clearing to alpha 0, AR Camera Manager is enabled and opaque scene geometry is not filling the view.
- Inspect the headset directly. The Game view may not include the compositor's passthrough layer; a black monitor background alone does not establish failure.

If Link still fails, an Android build on the headset is a useful independent test. It uses the existing Android feature configuration; the PC-only HD render override does not run there.

This provides a camera-based approximation of wearing AR glasses. It cannot reproduce an optical see-through display's light transmission, field of view or optics exactly.

## Verification

Follow-up fix: the template Environment.fbx instance was still enabled and covering the camera background. It is now inactive in PersonalARPrototype, with its old passthrough fade callback disabled. The template GoalManager component is also explicitly disabled as a safeguard; its parent was already inactive, so it was not established as the cause of this failure. Latest inspected logs advertised XR_FB_passthrough and reached FOCUSED. These confirm runtime support/session availability, not visual passthrough success.

PersonalAR scripts compile against the installed Unity assemblies. Live passthrough, room contrast and comfort remain to be tested in the headset. The platform switches have not been changed by the agent.
