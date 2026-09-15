# View modules outside Play Mode

Open PersonalARPrototype and choose **PersonalAR > HUD Preview** from Unity's top menu. Alternatively, select **PersonalAR / HUDAnchor / HUD**, then click **Open HUD Preview** on the Modular Hud component.

The dockable window renders an isolated copy using the same module layout builder as Play Mode. It uses sample values and does not start an XR rig, change the saved scene, or change the live HUD's settings.

- **Focus:** show the normally hidden center module.
- **Glass:** compare translucent panels and controls.
- **Library:** inspect the module library.
- **Test background:** check contrast against different solid colors.
- **Refresh:** rebuild after changes. If auto-detection fails, assign the scene's HUD component to HUD source.

This is a visual inspection window. Module buttons, head tracking, passthrough and real task interactions still require Play Mode. The preview is cleaned up when closed or when entering Play Mode; it can rebuild when returning to Edit Mode. Layout changes still live in ModularHud's builder, so this is not a drag-and-drop scene authoring system.

Validation: both runtime scripts with UNITY_EDITOR enabled and the editor window compile against the installed Unity assemblies. Actual Editor rendering still needs a visual check.
