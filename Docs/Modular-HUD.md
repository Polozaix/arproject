# Modular HUD overhaul

**Interaction update:** Date is now part of `clock`; the bottom-left position holds the permanent Modules / Undo dock. Every built-in module can be dismissed and restored through the library, with settings for title and opacity plus module-specific controls. See [HUD-Visual-Plan.md](HUD-Visual-Plan.md) for current behavior, limitations and the next graphical pass. The original layout below records the first overhaul.

Open `Assets/Scenes/PersonalARPrototype.unity`, reload the scene if Unity has an older in-memory version, and enter Play Mode. Keep unrelated unsaved scene work separately before reloading. The scene is Build Settings scene 0, so it is also what an Android/Quest build launches.

## Layout

The shared 1000 × 600 world-space canvas follows the head as one tab. The sketch is interpreted as an attention hierarchy:

| Level | Positions | Default modules |
| --- | --- | --- |
| 1 | Four corners | Identity, local clock, date, demo session timer |
| 2 | Left / right | Objective progress and module controls |
| 3 | Top / bottom | Workspace heading and session context |
| 4 | Center | Current objective, explicitly opened with Show focus |

Clipped dark panels, thin turquoise rims, muted secondary text and restrained accent seams replace flat panel backgrounds. No texture packages or simulated sensor readings are used. The center starts empty. This is a design hypothesis; placement alone does not establish measured attention or glanceability.

**Open tasks** opens the existing world-placed objective list. Detail cards still support moving, flick dismissal and completion. **Show focus** deploys the central summary, **Context on / off** toggles the top and bottom together, and **Recenter HUD** resets the shared follow anchor. Task progress and elapsed time are session-only.

## Module API

`ModularHud` owns the nine slots. `HudModule.Attention` derives the tier from the slot. All deployed modules inherit the same motion; they do not each chase the camera.

After the HUD's Start has run, another component can replace a built-in view with an instantiated UI prefab:

```csharp
hud.Unregister("workspace");
RectTransform view = Instantiate(myModulePrefab);
if (!hud.Register("navigation", HudSlot.Top, view))
    Destroy(view.gameObject);
hud.SetDeployed("navigation", true);
```

Register rejects duplicate IDs, invalid slots and occupied slots. Unregister destroys the old view and frees its slot. Hidden modules retain their slots; use Unregister to replace them. Prefabs own their content, input and data subscriptions, and should release subscriptions when disabled/destroyed. This is in-process UI deployment, not downloaded code or a remote plugin system. Custom modules must fit their allocated rectangle. The shell currently targets the saved 1000 × 600 canvas.

Built-in IDs: `identity`, `clock`, `date`, `session`, `workspace`, `objectives`, `controls`, `focus`, `activity`.

## Motion

`HUDAnchor/SoftHeadFollow`: distance 1.2 m, position smooth time 0.20 seconds, rotation sharpness 8, maximum angular lag 12 degrees. The follower keeps the shell at its viewing radius, bounds direction and rotation lag, and immediately places it on initial activation or recenter. Unscaled time keeps motion independent of time scale. These replace the earlier 0.07 / 16 starting values described in HUD-Controls.md.

## Validation and demo rehearsal

All eleven PersonalAR scripts (ten runtime scripts plus the editor preview window) compile against the project's Unity assemblies with Unity's bundled C# compiler. Serialized-field warnings are expected outside Unity; scene references are assigned. Scene IDs and component wiring were checked. This is not an Editor Play Mode or headset rendering verification.

Before presenting:

1. Reload the scene, confirm a clean Unity Console and test mouse/controller button highlighting and activation.
2. Confirm the clock appears only once, modules do not overlap, and center starts clear.
3. Toggle focus/context; open an objective, complete it, and check the HUD progress and next objective update.
4. Move and dismiss a detail card, then reopen it; check completion is retained.
5. Try slow turns, fast turns, looking up/down, translation and Recenter. Adjust smoothing in the headset if the panel feels distracting.
6. Check text and panel contrast against both bright and dark surroundings, with all modules visible. Re-enable the HUD and confirm state is retained.

Headset readability, comfort, frame rate and passthrough appearance remain unverified for this revision.
