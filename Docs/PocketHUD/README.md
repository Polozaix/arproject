# PocketHUD — contextual embedded display prototype

**Status (2 November 2026):** proposal + interactive **browser-only UI simulator**. **No Elecrow hardware has been received or tested.**

PocketHUD explores how a compact, glanceable interface can show a few high-priority items from a computer or phone without becoming another distracting screen. It is a small embedded-interface experiment connected to this repository's existing Unity/Quest HUD work, not a finished wearable or optical waveguide.

## Why this builds on PersonalAR

This repository already has a modular HUD prototype in Unity. See [Modular HUD](../Modular-HUD.md), [HUD visual plan](../HUD-Visual-Plan.md) and [testing/observations](../Testing-and-Observations.md). Those documents describe implemented interactions alongside clearly labelled untested ideas. PocketHUD takes a deliberately narrower part of the problem—readable, interactive glanceable content—and explores it on inexpensive microcontroller hardware.

## Try the current browser proof of concept

1. Open [preview.html](preview.html) locally in a modern browser; no installation, accounts or network connection required.
2. Click **Next card** or use the Left/Right arrow keys to cycle through demonstration cards.
3. Use the **Contrast** button to test a simple visibility tradeoff.
4. Enter a custom message and select **Queue demo message**; this is simulated locally.
5. Toggle **Round layout** to compare how much information fits a smaller display concept.

The preview is **a UI simulator only**. It does **not** communicate with an ESP32, send real phone notifications, display images inside glasses, capture camera input, or measure readability. It uses synthetic demonstration data, and it never uploads information.

## Proposed physical build

```text
PC (initially manual/test events) 
     │ local Wi-Fi, later optional authenticated companion
     ▼
ESP32-S3 + 2.4-inch CrowPanel touchscreen
     │
     ├── task / notification / timer cards
     ├── touch navigation, acknowledge/dismiss
     └── logging for test measurements (later)
```

Request: one **Elecrow CrowPanel Advance 2.4-inch ESP32-S3 HMI** (SKU DIS01624A-1), [official product page](https://m.elecrow.com/pages/shop/product/details?id=207959).

Optional, only if available: one **Elecrow CrowPanel 1.28-inch round ESP32-C3** (SKU DIS12824D), [official product page](https://www.elecrow.com/crowpanel-esp32-display-1-28-r-inch-240-240-round-ips-display-capacitive-touch-spi-screen.html). It enables a second form-factor study. The main project works with the 2.4-inch unit alone.

Both are LCD-based development boards, **not AR microLED waveguide modules**. They allow firmware and UI principles to be tested before moving to optical see-through hardware.

## Planned milestones

| Stage | Goal | Evidence |
| --- | --- | --- |
| 0 — current | Browser UI concept with synthetic input | [Interactive preview](preview.html), this README |
| 1 — after hardware | Board bring-up, versioned firmware, basic cards | Build log and code; test photos |
| 2 | Local Wi-Fi demo-event transport and on-device navigation | Short reproducible demo and event logs |
| 3 | Compare spacing, contrast, text length and glance-time tasks | Test protocol, raw results, limitations |
| 4 | Public tutorial and honest assessment | Repository, Elecrow Community post and optional short video |

The dates depend on when/if hardware is available. Nothing in stages 1–4 is claimed as completed.

## Sponsorship and editorial independence

A sample, if provided, will be disclosed at the top of the project write-up and community post. Positive coverage is **not guaranteed**: usability issues, setup problems and tradeoffs will be reported. No private phone notifications or personal data will be published.

See [PROJECT_PROPOSAL.md](PROJECT_PROPOSAL.md) for scope, deliverables, risks, test plan and links.

**Author:** Tarik Hamza Bilgin — final-year BSc (Hons) Software Engineering, De Montfort University, Leicester (independent student project; not an official DMU partnership).
