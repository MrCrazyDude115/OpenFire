# Open Fire

**Unity · C# · Solo university project · 4 weeks**

A short, *Time Crisis*-inspired arcade shooting prototype built around peeking from cover, reacting to dangerous enemy shots, and clearing scripted combat encounters. The project contains a single proof-of-concept level rather than a full game.

## My role

**Solo developer — gameplay programming and technical design.** I implemented the player shooting and cover loop, enemy behaviours, configurable encounters, and camera transitions.

## Technical highlights

- **Cover-based shooting:** Right-click to peek out and shoot; releasing returns the player to cover and reloads. Shooting uses camera-based raycasts, an ammo counter, and hit feedback (`PlayerCoverController.cs`, `PlayerShooter.cs`).
- **Configurable encounters:** `ArenaController.cs` exposes enemy spawn configurations and waves in the Unity Inspector, including enemy behaviour options and kill thresholds that can trigger overlapping waves.
- **Enemy attack behaviours:** `EnemyCoverShooter.cs` supports different exposure, weapon, and firing setups, including cover pop-ups and telegraphed **DANGER** shots that the player must avoid.
- **Camera and area transitions:** Cinemachine cameras move the player between combat areas. `AreaTransitionController.cs` temporarily disables shooting/cover controls and protects the player during transitions.

## Controls

| Input | Action |
| --- | --- |
| Mouse | Aim |
| Left mouse button | Shoot while peeking |
| Hold right mouse button | Peek out of cover |
| Release right mouse button | Return to cover and reload |

## Project scope

Developed over **four weeks** for a university assignment. The goal was to prototype the core feel of a classic arcade cover shooter in **one short level**, not deliver a complete commercial game.

## What this project demonstrates

**C# gameplay programming · configurable encounter design · enemy behaviour logic · player-state coordination · Cinemachine integration**

The focus was getting several connected systems—player actions, enemy attacks, wave progression, and camera movement—to work together.

## Media and links

- **[Add your portfolio page link]** — detailed breakdown, screenshots, and *The Making of Open Fire* development video (**needs your input**).
- **[Add a gameplay video or playable build link]** if one is publicly available (**needs your input**).

> **Asset attribution:** Add a short note identifying any third-party art, animation, or audio packages used in the showcased build (**needs your input**).
