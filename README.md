# Open Fire

**Unity - C# - Solo university project - 4 weeks**

A short, *Time Crisis*-inspired arcade shooting prototype built around peeking from cover, reacting to dangerous enemy shots, and clearing combat arenas. The project contains a single proof-of-concept level, not a full game.

**[Play Open Fire on itch.io](https://apexgenesis.itch.io/open-fire)**
## My role

**Solo developer - gameplay programming and technical design.** I implemented the player shooting and cover loop, enemy behaviours, configurable encounters, and camera transitions.

## Technical highlights

- **Cover-based shooting:** Hold down Right-click to peek out and shoot. Releasing returns the player to cover and reloads. Shooting uses camera-based raycasts, an ammo counter, and hit feedback (`PlayerCoverController.cs`, `PlayerShooter.cs`).
- **Configurable encounters:** `ArenaController.cs` allows the editing of enemy spawn configurations and waves in the Unity Inspector, including enemy behaviour options and kill thresholds that can trigger overlapping waves.
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

Developed over **four weeks** for a university assignment. My goal was to prototype the core of a classic arcade cover shooter in one short level.
