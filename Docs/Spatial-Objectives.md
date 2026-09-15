# Spatial objectives prototype

The `SpatialObjectives` component on the HUD creates the launcher and two reusable world-space panels at runtime. Disable this component before Play Mode to remove the feature without removing the clock or optional frame. Sample state is stored separately in `ObjectiveState`; no accounts or backend are used.

In the prototype scene, `SpatialObjectives` and `ModularHud` are components of the same `HUD` object, so the lower-centre `OBJECTIVES` launcher is hidden and the objectives module supplies the entry points instead. The launcher only appears when this component runs without a `ModularHud` on the same object.

## Controls

1. Enter Quest Link and start `PersonalARPrototype` in Unity.
2. Point a controller ray at **Open tasks** on the left-hand `01 / OBJECTIVES` module of the HUD (or open **Modules > Open tasks**) and press the UI select trigger. The focus module's **View objective** goes straight to the next incomplete task.
3. Select one of three sample objectives to read its detail card.
4. Drag the card by holding its header with the UI select trigger. Movement is along the card's opening plane; this is ray dragging, not a physics grab with the grip button.
5. Release slowly to leave it stationary. A fast header flick dismisses the view; this experimental threshold needs headset tuning.
6. X closes the card. MARK COMPLETE changes task state; MARK INCOMPLETE reverses it. Closing never deletes or completes the task.
7. Use **Open tasks** again, or the OBJECTIVES button on a detail card, to reopen the list in front of you; this provides recovery if a card was placed out of reach.

Mouse input in Game view is also supported for development. Only one list or detail card is shown at a time. A new opening recentres the panel; this first version does not remember panel positions after closing. Completion lasts for the current Play Mode session and resets on restart. Personal task editing and storage are later additions.

## Validation checklist

- Launcher opens all three objective rows.
- The lower-centre launcher is hidden while `ModularHud` is present, and **Open tasks** on the objectives module opens the same list (verified against the scene structure on 2026-09-15; controller targeting still needs a headset pass).
- Each row shows its own title and description without clipped text.
- Completing then closing/reopening retains completion within the session.
- Marking incomplete reverses completion and updates the count.
- Closing or tossing does not change completion.
- Slow header dragging leaves a stationary panel; opening Objectives recovers it.
- No new repeated application exceptions occur.
- Test controller hover/select and flick behaviour in the physical headset before recording them as verified.

The existing template may report missing AR subsystems if Quest Link is unavailable. Those warnings should be distinguished from errors in this feature.
