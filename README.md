# Mage Battle Arena

## Enemy Bot Upgrade TODO

- [x] Add an attack telegraph during wind-up, such as a glow, aim line, color pulse, or charging VFX.
- [ ] Improve range discipline so the enemy chases when too far, holds at ideal range, and backs away when too close.
- [ ] Add aim commitment near the end of wind-up so the player has a fair dodge window.
- [ ] Add simple attack variation, such as alternating between a single shot and a short burst.
- [ ] Add animation and VFX hooks for idle, move, wind-up, shoot, and death states.
- [ ] Add death feedback, such as a small dissolve, pop, or particle burst before respawn.

Mage Battle Arena is a small Unity 3D arena battle prototype with mobile joystick movement, aiming, projectile combat, enemy NavMeshAgent movement, health/damage, respawn logic, timer UI, and a basic arena scene.

## Unity Version

- Target Unity version: Unity 6000.4.10f1 / Unity 6.4.
- Original project version: Unity 2019.1.8f1.
- The project has been opened and confirmed working on Unity 6.4, so the codebase now targets Unity 6.4 only. No Unity 2019 compatibility paths are kept.

## Current Status

- Main test scene: `Assets/Scenes/SampleScene.unity`.
- Main gameplay loop: player and enemy can move, aim, shoot, take damage, die, respawn, and reach the timer end state.
- Input remains based on the bundled Virtual Joystick Pack and the legacy Input Manager.
- UI remains based on Unity UI `Text`, `Image`, and `Slider` components.

## Cleanup Pass Completed

- Removed the old observer layer and moved damage flash / timer updates into direct runtime flows.
- Removed the old `Player` and `EnemyBot` wrapper classes that held scene objects as plain C# state.
- Kept `NormalAttack` as a compact projectile attack data object.
- Refactored `PlayerController` around explicit serialized references, cached Rigidbody/joystick/UI references, direct health handling, direct damage flash updates, queued shooting, and single-shot respawn requests.
- Refactored `EnemyController` around explicit serialized references, cached Rigidbody/NavMeshAgent references, throttled path updates, direct health handling, direct projectile shooting, and single-shot respawn requests.
- Refactored `GameController` to use a clearer `Instance` singleton, direct match timer logic, cached respawn delay, and separate player/enemy respawn methods.
- Refactored `ProjectileBehaviour` so projectiles are configured with damage/range at spawn time and use squared distance checks for range cleanup.
- Updated the floating joystick bridge so it no longer reaches through controller internals or toggles public shoot flags.
- Wired `SampleScene` references for the player joysticks, enemy target, timer text, and match-end panel.

## How To Open

1. Open Unity Hub.
2. Open this project with Unity 6000.4.10f1.
3. Open `Assets/Scenes/SampleScene.unity`.
4. Let Unity finish importing and compiling scripts.

## How To Test SampleScene

1. Press Play in `Assets/Scenes/SampleScene.unity`.
2. Move the player with the left joystick.
3. Aim and shoot with the right joystick.
4. Confirm the enemy follows the player using the baked NavMesh.
5. Confirm player and enemy projectiles apply damage.
6. Confirm health sliders update.
7. Confirm player damage flashes the damage image.
8. Confirm player and enemy death each trigger one respawn.
9. Confirm the timer reaches `01:00`, shows `TimeUpPannel`, and pauses the match.
10. Stop Play Mode and press Play again to confirm `Time.timeScale` resets.

## Known Remaining Issues

- The project still uses legacy Unity UI `Text` instead of TextMeshPro.
- The project still uses the legacy Input Manager and bundled joystick package.
- There are old generated Unity files and package-cache changes in the working tree from prior imports; they should be reviewed separately from gameplay code.
- The baked NavMesh should be rechecked after major arena or Unity lighting/navigation changes.
- There are no automated PlayMode tests yet for movement, shooting, respawn, or timer behavior.

## Recommended Next Tasks

- Run a full Unity Editor compile and playthrough on `SampleScene`.
- Add PlayMode tests for health, projectile damage, respawn, and timer pause behavior.
- Move tuning values into ScriptableObjects once the gameplay loop stabilizes.
- Consider replacing legacy UI `Text` with TextMeshPro.
- Review and remove unused packages through Unity Package Manager.
