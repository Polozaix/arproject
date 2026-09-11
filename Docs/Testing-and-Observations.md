# PersonalAR testing and observations

Use this notebook to record what was tested, what happened, and what evidence supports the next design decision. It covers development on Quest 2 and can grow into material for the final-year report. Blank fields mean not recorded, not a successful result.

## How to use this notebook

Copy the session template into `Docs/TestSessions/YYYY-MM-DD-short-name.md` for each session. Keep screenshots, recordings, and exported measurements in a matching evidence folder, or link to their actual location. Use relative links where possible. Do not overwrite earlier results after changing the software.

Keep observations separate from interpretations. “Text crossed the centre of the view on 3 of 5 turns” is an observation. “The following delay may be too large” is an interpretation. “It felt better” is useful feedback when accompanied by the condition, task, and reason.

Short sessions are enough for development. Record failures and abandoned approaches as carefully as successes. Mark all mock notifications, simulated inputs, estimates, and unmeasured quantities explicitly.

## Recorded sessions

- [HUD-001: HUD size exploration on Quest 2](TestSessions/2026-09-09-hud-size-exploration.md) — developer preference for an intermediate layout; qualitative exploratory evidence.

## Session template

### Identification and purpose

- Session ID and date:
- Tester ID (use SELF for developer testing):
- Test type: functional / usability exploration / performance / regression
- Feature and question being investigated:
- Hypothesis or expected behaviour:
- Decision this test should inform:
- Status: planned / completed / interrupted / blocked

### Reproducible setup

- Git commit and branch; describe any uncommitted changes:
- Build ID, scene, and Unity version:
- Device and runtime/OS version, where known:
- Execution: standalone Quest build / PC connection / Editor simulation
- Input method:
- Passthrough or virtual environment:
- Seated or standing; room conditions and relevant background contrast:
- Starting headset battery and session duration:
- HUD hierarchy and parent transform (especially whether parented to the camera):
- Components enabled and assigned head/camera reference:
- Panel placement, distance, scale, text size, colour, and opacity:
- Position smoothing, rotation smoothing, and any dead zone:
- Data source: live / local stored / mock / simulated
- Other relevant settings or differences from the previous session:

Save an Inspector screenshot if many settings are involved. A script's default values do not establish the values used in a saved scene or running build.

### Procedure and success criteria

1. Starting state and reset procedure:
2. Exact user task:
3. Actions or movement sequence:
4. Condition order and repetitions:
5. Measurements and collection method:
6. Success criteria chosen before testing:
7. Reasons to stop or mark a trial invalid:

For comparisons, keep unrelated settings and content fixed. Change one variable at a time initially. Alternate or vary condition order and record it so practice and fatigue are visible. Record interrupted trials and reasons rather than quietly discarding them.

### Trial record

| Trial | Condition/settings ID | Task | Success and errors | Time, if measured | Observation | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | | | | | | |
| 2 | | | | | | |
| 3 | | | | | | |

Define timing start and end points before collecting task times. Record units and the tool used. Do not describe a visual impression of smoothness as measured latency or frame rate.

### Immediate feedback

These are project-specific development ratings, not a validated questionnaire. Keep their wording and anchors unchanged when comparing sessions. Record N/A when an item was not assessed.

| Item | Rating from 1 to 5 | Explanation or specific incident |
| --- | --- | --- |
| Text readability | 1 = very difficult, 5 = very easy | |
| Perceived visual stability | 1 = very unstable, 5 = very stable | |
| Distraction | 1 = none, 5 = very distracting | |
| Discomfort | 1 = none, 5 = severe | |

- Preferred condition and why:
- Unexpected behaviour:
- Symptoms and onset time, if any; whether the session stopped:

### Results and decision

- Observed facts, including failed trials:
- Summary counts with denominators (for example, 4/5 successful):
- Interpretation and possible alternative explanations:
- Limitations: tester count, developer familiarity, simulation, missing measurements:
- Outcome against each success criterion: pass / fail / inconclusive
- Decision: keep / adjust / revert / investigate
- Change proposed and reason:
- Next test:
- Evidence links and related issue/commit:

Do not generalise a developer preference into a finding about all users. A short indoor test does not establish suitability for outdoor walking or cycling.

## First experiment: clock following and slight sway

Status: proposed; no results collected.

### Motivation and question

Initial user observation: the clock feels stiff. The desired change is slight movement that makes it feel less rigid. Compare rigid placement with gentle movement caused by head motion, while preserving readability and predictable placement.

Question: Does a small following delay reduce perceived stiffness without making the clock drift, distract, or become harder to read?

The current `Assets/PersonalAR/Scripts/XR/SoftHeadSway.cs` declares `SoftHeadFollow` and smooths movement toward the head's pose. It does not generate a periodic sway animation. Its source defaults are distance 1.2 m, position smooth time 0.10 s, and rotation sharpness 12. These are code defaults, not verified runtime settings.

### Prerequisites

- Match the script filename and component class name.
- Verify that the component is attached, enabled, and assigned the intended head transform.
- Record the transform hierarchy. Check that camera parenting is not causing immediate movement that defeats the intended following behaviour.
- Verify the baseline and each comparison mode in the actual headset.
- Keep a way to hide the HUD and stop the test immediately if uncomfortable.

### Conditions

Use these as exploratory starting values, not established comfort recommendations. Keep panel distance, text size, content, and placement fixed.

| ID | Condition | Position smooth time | Rotation sharpness |
| --- | --- | --- | --- |
| A | Rigid head-relative baseline | Not applicable | Not applicable |
| B | Gentle following | 0.05 s | 12 |
| C | More position delay | 0.10 s | 12 |

Implement A explicitly as rigid placement; do not assume a particular smoothing value reproduces it. First compare B and C to isolate position smoothing. If rotation still feels stiff, retain the preferred position value and separately compare rotation sharpness 12 and 20. In the current formula, a higher rotation sharpness follows rotation faster.

Avoid adding an idle oscillation in this first experiment: it introduces a separate variable. If deliberate sway is wanted later, evaluate it as a separate condition with recorded amplitude and frequency.

### Procedure

1. Begin seated in a clear indoor area. Record the build and all condition settings.
2. Try each condition briefly to understand the task; label this as practice.
3. Hold the head still and read the clock. Observe whether the panel continues moving.
4. Make a slow, comfortable left/right head turn, stop, and read the clock again.
5. Repeat with a small up/down head movement. Avoid forced ranges or rapid movements.
6. Perform five trials per condition, varying condition order and recording the actual order.
7. Rate readability, stability, distraction, and discomfort immediately after each condition. Add a separate stiffness rating: 1 = not stiff, 5 = very stiff.
8. Record preference and the specific reason. Take breaks as needed and record them.

### What to observe

- Is the time read correctly on each attempt?
- Does the clock keep drifting after the head stops?
- Does it cross the central view unexpectedly or leave the intended visible area?
- Is there jitter, snapping, or an initial jump when shown?
- Does softer following feel pleasant, delayed, or detached from the user?
- Does the user need to wait for it to settle before reading?
- Does hiding and restoring it produce a predictable position?

Use a changed test time or other unfamiliar short text if assessing reading accuracy; repeatedly reading the same known time can hide readability problems. Label such content as test data. Keep the live-clock functional check separate.

### Decision rule

Treat this as a personal design exploration. Prefer a condition only if stiffness improves without a noticeable loss of reading success or an increase in distraction/discomfort. If results conflict or the difference is unclear, record the outcome as inconclusive and retain the simpler baseline pending another test. Do not select a condition solely because its motion looks attractive in a recording.

## Future test checklist

| Area | Test question | Evidence to keep |
| --- | --- | --- |
| Deployment | Can the same build procedure be repeated? | Build steps, device launch result, errors |
| Clock | Does local time display and update correctly? | Compared time source, minute transition, pause/resume result |
| Placement | Do follow and stationary modes behave as intended? | Settings, mode transitions, movement observations |
| Notifications | Can an item be noticed, opened, and dismissed? | Success counts, accidental actions, task times |
| Reading | Can unfamiliar content be read at the chosen size? | Content ID, accuracy, size/distance, feedback |
| Input | Can the same task be completed using each input method? | Method, errors, completion time, preference |
| Persistence | Do settings survive a full app restart? | Before/after values and build ID |
| Recovery | What happens after tracking loss or app interruption? | Trigger, recovery behaviour, lost state |
| Performance | Does a feature affect rendering performance? | Device profiler capture, workload, duration, frame-time summary |
| Simulated navigation | Do known routes/headings produce expected cues? | Inputs, expected versus actual output, coordinate conventions |

## Evidence for the eventual report

Maintain a small index as work progresses. An honest negative result can explain an important design decision.

| Claim or decision | Requirement/question | Session/evidence | Limitation | Report topic |
| --- | --- | --- | --- | --- |
| To be filled after testing | | | | |

Keep dated notes on requirements, architecture decisions, alternatives considered, implementation challenges, test results, and remaining limitations. Record literature references when you actually read them, including what claim they support. Do not manufacture retrospective results to fill gaps.

Developer testing can begin now. Before recruiting other participants, confirm the course's supervision, consent, and ethics process. Store participant identities separately from test records if needed, and avoid recording private messages in demonstrations.
