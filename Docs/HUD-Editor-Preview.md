# View modules outside Play Mode

Open PersonalARPrototype and choose **PersonalAR > HUD Preview** from Unity's top menu. Alternatively, select **PersonalAR / HUDAnchor / HUD**, then click **Open HUD Preview** on the Modular Hud component.

The dockable window renders an isolated copy using the same module layout builder as Play Mode. It uses sample values and does not start an XR rig, change the saved scene, or change the live HUD's settings.

- **Focus:** show the normally hidden center module.
- **Glass:** compare translucent panels and controls.
- **Library:** inspect the module library.
- **Test background:** check contrast against different solid colors.
- **Refresh:** rebuild after changes. If auto-detection fails, assign the scene's HUD component to HUD source.

This is a visual inspection window. Module buttons, head tracking, passthrough and real task interactions still require Play Mode. The preview is cleaned up when closed or when entering Play Mode; it can rebuild when returning to Edit Mode. Layout changes still live in ModularHud's builder, so this is not a drag-and-drop scene authoring system.

Validation: both runtime scripts with UNITY_EDITOR enabled and the editor window compile against the installed Unity assemblies. Rebuilt with `dotnet build Assembly-CSharp.csproj` against the Unity 6000.5.8f1 assemblies on 2026-09-15: 0 errors, with five pre-existing warnings in template/sample code only. Actual Editor rendering still needs a visual check.

This window shows the same layout builder used at runtime, so the Hierarchy names it produces are the ones described in [Modular-HUD.md](Modular-HUD.md#inspecting-and-editing-the-runtime-hud-in-play-mode). Those generated objects exist only in Play Mode; the preview's own copy lives in an isolated preview scene and is deliberately not editable.
