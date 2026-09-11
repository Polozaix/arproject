# HUD size exploration on Quest 2

## Session record

- Session ID: HUD-001
- Date: 2026-09-09
- Tester: SELF, the project developer; one participant
- Method: informal, formative self-evaluation with iterative adjustment
- Status: completed exploratory observation; repeatability testing pending
- Question: Which HUD dimensions feel comfortable for viewing the clock while retaining a visible decorative frame?
- Evidence sources: the developer's feedback in the project conversation, observed Unity Inspector values, saved scene configuration, and Unity's XR startup log. This note was written after the observations, rather than from a prespecified experiment.

## Setup

- Hardware: Meta Quest 2 connected by cable through Meta Horizon Link.
- Software: Unity 6000.5.8f1; `Assets/Scenes/PersonalARPrototype.unity` in Editor Play Mode, using the desktop OpenXR provider.
- HUD: world-space canvas under `PersonalAR/HUDAnchor`; clock anchored top-right; optional cyan tactical frame.
- Observed final Inspector canvas scale: 0.001 on all axes.
- Saved follow configuration: distance 1.2 m, position smooth time 0.07 s, rotation sharpness 16. These values were established earlier in the session; they were not independently measured throughout every user adjustment.
- Clock: saved font size 36, rectangle 160 by 60 units, right margin 32 and top margin 28 units.
- Unity's left-eye mirror showed a virtual sky background. Passthrough viewing was not established.
- Posture, duration, breaks, full sequence of intermediate settings, and number of repetitions: not recorded.
- Git base: `ca93be6ad51706fa3a8a871a5049eded65c03492` on `main`. Tested software includes uncommitted changes. A Git base revision alone does not reproduce the tested build.

## Observations

| Condition | Canvas width by height | Recorded feedback | Evidence type |
| --- | --- | --- | --- |
| Initial compact layout | 600 by 300 | Developer confirmed the headset display worked and described it as great, but wanted a larger HUD nearer the view edges. | Conversation and earlier configuration |
| Expanded layout | 1600 by 900 | Clock and borders were visible, but looking toward the extreme corner felt uncomfortable. Developer subsequently clarified that seeing the border was pleasant and not the concern. | Conversation and saved scene |
| Preferred layout after manual tuning | 1000 by 600 | Developer reported this was the most comfortable of the values tried. | Self-report plus final Inspector observation |

The message reporting the preferred size said "100 width and 600 height". The current HUD Inspector showed Width 1000 and Height 600. This record uses the directly observed 1000 by 600 values, treating the message's width as an apparent typo; it preserves the discrepancy rather than silently altering the source.

Selected verbatim feedback:

> "i can see the clock and the borders but its really uncommfortable to look at"

> "also seeing the border is nice i dont mind it"

> "feels the most comfortable"

No numeric comfort rating, reading accuracy, gaze angle, task time, or physiological measure was collected. Do not infer these from the feedback or assign retrospective scores.

## Interpretation and decision

The developer preferred an intermediate canvas size over the initial compact and expanded layouts in this session. The comments suggest that accessing the clock near the extreme corner was the concern, rather than the mere presence of decorative borders.

Because the clock is anchored to the canvas corner, changing canvas dimensions changes both the frame extent and clock position. This test therefore does not isolate canvas size from clock eccentricity, following behaviour, or other perceptual effects. The proposed explanation is a design hypothesis, not a demonstrated cause of discomfort.

Adopt 1000 by 600 as the provisional preferred setting for the next comparison. Retain the optional frame. Consider separating clock placement from frame extent if future tests reveal the same issue.

At scale 0.001 and distance 1.2 m, a centred 1000 by 600 canvas is 1.0 by 0.6 m and spans approximately 45.2 by 28.1 degrees. These are geometric estimates using `2 * atan(dimension / (2 * distance))`, not eye-tracking or comfort measurements. They assume the saved distance was maintained.

At the time of recording, the live Editor Inspector showed 1000 by 600 outside Play Mode, while the scene file on disk still contained 1600 by 900. The preferred dimensions were not verified as saved. No scene changes were made as part of writing this research note.

## Limitations and next test

This is evidence of one developer's preference in a particular prototype. It does not establish a universally comfortable size, prolonged-use comfort, or suitability for walking/cycling. The developer knew the design intent, selected settings adaptively, and did not use randomised conditions or a standardised task.

For a follow-up, record and save the exact settings, then compare the preferred layout with nearby alternatives using unfamiliar clock values or short text. Hold distance, font size, smoothing, background, and input method fixed. Record actual condition order, trial counts, reading success, and immediate comfort/readability ratings. Separately vary clock position while holding frame size fixed to investigate the suspected cause.

## Provisional report wording

During an informal formative evaluation on Quest 2, the developer iteratively adjusted the HUD canvas dimensions. The expanded 1600 by 900 layout allowed the clock and borders to remain visible, but viewing the clock near the outer corner was reported as uncomfortable. The developer distinguished this from the decorative border, which remained desirable, and preferred a 1000 by 600 layout after further adjustment. This preference informed the next prototype configuration. As a single-person exploratory evaluation without controlled trials or objective performance measures, the result supports a local design decision rather than a general claim about optimal AR HUD dimensions.
