# PocketHUD — Student project brief and Elecrow evaluation request

**Project owner:** Tarik Hamza Bilgin, final-year BSc (Hons) Software Engineering, De Montfort University, Leicester, UK.

**Project type:** independent educational / open-source learning project. No institutional endorsement or grant is implied.

**Current state, 2 November 2026:** the linked repository contains an existing Unity XR / modular HUD project and a PocketHUD browser simulation. This document is a proposal. No Elecrow electronics have yet been received and no hardware results are asserted.

## 1. Problem and objective

Most compact displays are constrained by screen area, limited input and power. For a hands-free or glanceable interface, the design question is not how much information can be shown, but which information remains readable and actionable at a glance.

**Research question:** How can a small embedded display surface three types of contextual information (a short task, a status/event and a timer) with minimal visual distraction and with a reliable dismissal/navigation action?

A low-cost ESP32 display allows repeatable hands-on experiments without claiming to fabricate AR waveguide optics. The results can inform future head-mounted display interfaces and this repository's Unity XR modules.

## 2. Proposed features and data flow

- **MVP:** manually authored/demo task, reminder and timer cards rendered on the CrowPanel. No camera or personal-data access required.
- **Device input:** touchscreen to move between cards and acknowledge a card.
- **Companion transport (planned):** a local PC application sends short JSON events over Wi-Fi to the ESP32. Any remote connection must have basic token authentication and rate limiting before use beyond a private test network.
- **Visual variants:** high/low contrast, concise/verbose text, optional dimming/night mode. The browser prototype lets these concepts be inspected before hardware is available.
- **Form-factor experiment:** compare rectangular display to a compact round alternative if a second board is provided.

Example **planned** event schema (subject to firmware implementation):

```json
{
  "version": 1,
  "id": "demo-001",
  "type": "task",
  "title": "Review coursework",
  "detail": "10-minute focus session",
  "created_at": "2026-11-02T17:00:00Z",
  "ttl_seconds": 600
}
```

## 3. Requested hardware and official Elecrow links

**Priority A — sufficient by itself**

- **1 × CrowPanel Advance 2.4-inch ESP32-S3 HMI Display**, SKU **DIS01624A-1**, [official product page](https://www.elecrow.com/crowpanel-advance-2-4-hmi-esp32-320x240-ips-ai-display-artificial-intelligent-touchscreen-1.html).
- Why: 320×240 IPS touchscreen, integrated ESP32-S3, Wi-Fi/BLE and LVGL/Arduino compatibility. Lets one board cover rendering, communication and input. As listed online around 2 Nov 2026: **from US$19.90**; pricing and UK shipping subject to confirmation.

**Priority B — optional comparison sample**

- **1 × CrowPanel 1.28-inch round ESP32 display**, SKU **DIS12824D**, [official product page](https://www.elecrow.com/crowpanel-esp32-display-1-28-r-inch-240-240-round-ips-display-capacitive-touch-spi-screen.html).
- Why: 240×240 touchscreen with ESP32-C3, Wi-Fi/BLE and a more constrained canvas for comparing text truncation and interaction layouts. Online listing: **US$16.90** at research time.
- If only one sample is possible, the **2.4-inch board** is the preference; no custom fabrication, accessories or shipping subsidy is assumed.

**Documentation:** [CrowPanel 1.28-inch guide](https://www.elecrow.com/wiki/CrowPanel_ESP32_1.28-inch_Round_Display.html); [Elecrow Project Community](https://www.elecrow.com/sharepj/).

Note: These are embedded LCDs, **not** waveguides or transparent AR lenses. The project tests interaction and low-power display architecture, not diffractive optics performance.

## 4. Success criteria and evidence

| Question | Test procedure | Planned evidence |
| --- | --- | --- |
| Can the board reliably display updated cards? | Push 30 synthetic test events, log delivered/rendered/failed | Screenshot, serial logs, source |
| Is navigation understandable? | Perform repeatable next/previous/dismiss tasks | Task checklist, errors and observations |
| What affects glanceability? | Compare text length and contrast at controlled distances/lighting | Photos, conditions, self-rated readability; no population claims |
| What happens when connectivity fails? | Disconnect/reconnect Wi-Fi during a demo | Failure/recovery logs |
| Does a round layout change content constraints? | Optional second-board A/B content layout | Counts of clipped/truncated fields and sample images |

Results will clearly distinguish measurements from opinions. Early tests with a single developer cannot establish general usability or safety.

## 5. Delivery and community plan

**Within approximately four weeks of receiving a sample, subject to coursework and hardware compatibility**, the project owner aims to publish:

1. A public GitHub tutorial including wiring/firmware instructions and code under this repository.
2. A reproducible setup guide with honest photos/screen recordings and at least one test-results table, including failures.
3. An adapted tutorial on [Elecrow Project Community](https://www.elecrow.com/sharepj/), with an acknowledgement that hardware was provided by Elecrow.
4. If feasible, a **60–120-second short demo video** showing the working prototype and pointing to the repository. This is a goal, not an existing content channel or guaranteed deliverable.

A gifted sample will be clearly disclosed. There is no promise of a positive review, reach metrics, subscriber count, or endorsement by the university. Before posting personal or private data, it will be replaced with synthetic examples.

## 6. Risks and fallback

- **Hardware unavailable:** continue developing the browser simulation and protocol. No false claim of finished hardware.
- **Different firmware APIs/pin mappings:** rely on the exact manufacturer's wiki and examples for the delivered revision.
- **Wi-Fi/security:** prototype only on a trusted local network; avoid publicly exposing the ESP32 endpoint.
- **Privacy:** first version only accepts manual/synthetic events; opt-in integration later.
- **Screen limitations:** treat LCD performance as separate from optical see-through HUD performance.
- **Time constraints:** keep initial scope minimal; publish honest partial results if necessary.

## 7. Relevant existing work and links

- Existing AR/Quest modular HUD repository: [Polozaix/arproject](https://github.com/Polozaix/arproject).
- Current modular HUD implementation notes: [Docs/Modular-HUD.md](../Modular-HUD.md).
- Existing test-recording protocol: [Docs/Testing-and-Observations.md](../Testing-and-Observations.md).
- New PocketHUD simulated UI: [preview.html](preview.html).
- Project owner GitHub profile: [github.com/Polozaix](https://github.com/Polozaix).

I currently have a GitHub portfolio; I am **not** claiming an established YouTube channel, existing sponsorships, audience size or completed Elecrow tutorial.
