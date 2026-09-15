# HUD interaction and graphical direction

## Implemented interaction pass

- Every built-in module has an X button. Dismissal hides its view and retains its session state.
- A permanent Modules / Undo dock occupies the freed bottom-left corner. It stays available when all modules are hidden.
- Modules opens a library with visible/hidden status, Show / hide and Settings for each module. The library also offers Open tasks, Dismiss all and Restore all.
- Undo restores the most recent dismissal, including an objective list/detail card closed with X or a detail card dismissed by flick. Undo after Dismiss all restores the previously visible set.
- Date and time share one corner module. Settings include 12/24-hour time and date visibility.
- Every built-in module has editable heading text and panel opacity presets. The session timer can be restarted from its settings.
- Editing means contents/settings, per the user's clarification; no slot-swapping UI is introduced.

Text entry currently uses TMP's keyboard input. A headset-native keyboard has not been implemented or verified. Button settings work through the existing UI input route. Settings and visibility are retained while closing/reopening within the current session; persistence across application launches is a later step. Editing objective descriptions is also a separate content-editor task.

## Proposed look: quiet spatial glass

The four attention tiers describe what deserves attention, not a requirement to fill nine boxes. Keep empty space deliberate. Treat the sketch's corner marks as a framing cue rather than adding more instrument scales.

### 1. Reduce competing content

- Retain combined date/time and the recovery dock in quiet corner positions.
- Combine objective count and session progress into one task summary; remove the repeated session-context banner.
- Put identity/workspace naming in the library header instead of using an always-visible identity card.
- Move recenter and view preferences into the dock/library, freeing the current controls panel.
- Open the central task view explicitly. Never bring it forward just because the user glances across it.

Acceptance: the default view has fewer visible panels, and each persistent element has a distinct purpose.

### 2. Establish a shared visual system

- Slate panels, warm white primary text and desaturated secondary text.
- A single mint accent for selected items, focus and actionable controls.
- Amber reserved for an actual condition needing attention.
- Softer corners, a fine rim and generous internal spacing. Replace multiple clipped seams and decorative ticks with a simpler silhouette.
- Three consistent type roles: module label, primary value/title and supporting text. Restrict uppercase to short labels.
- Use the same controls on HUD modules, the library and detached task cards.

Implementation: centralize colors, spacing, typography and surface shape into a shared theme asset, then migrate the three view types. Compare actual headset rendering against bright and dark backgrounds before settling opacity and type sizes.

### 3. Communicate interaction clearly

- Keep dismiss targets reliably available; make them visually quieter when idle and distinct when pointed at.
- Use separate hover, pressed, selected and unavailable states.
- Show a short description of each setting and its current value. Label undo availability clearly.
- Keep the recovery dock in a stable position. A closed view must never remove its own recovery route.
- Add a headset keyboard or a deliberate companion editing flow before claiming effortless in-headset text editing.

Acceptance: rehearse dismiss, restore and edit without explaining hidden gestures. No gaze-triggered dismissal or confirmation dialogs for hiding a view.

### 4. Add restrained motion

- Try a short fade and a very small settling movement for open/close, initially around 120–180 ms.
- Keep controls stationary while pointed at or pressed so the target does not move away during selection.
- Keep the shell's existing shared follow motion; avoid independent wobble on every module.
- Provide reduced-motion and follow-strength settings.

These timing values are starting proposals, not measured comfort results. Confirm the interaction model first, then tune motion in the headset.

### 5. Validate for a demonstration

Rehearse: start with a quiet HUD, open tasks, complete one, dismiss its card, undo, hide all modules, reopen the library, restore a module and change its settings.

Check clean Console, controller targeting, text clipping, contrast, rapid dismiss/restore, hidden-module state, and reactivation. Test head turns and tracking recovery, then measure frame time with the representative visible module set. Save a repeatable demo configuration only after these checks.

## Verification of this interaction pass

The PersonalAR scripts compile with Unity's bundled C# compiler against the project's assemblies. This does not validate runtime UI rendering or headset comfort. A new Play Mode/headset pass remains required, particularly for the dock target size and keyboard entry.
