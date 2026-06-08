# Mage Battle Arena

Mage Battle Arena is an older Unity 3D arena battle prototype. The project currently contains player movement, mobile joystick controls, aiming, projectile combat, enemy NavMeshAgent movement, health/damage, respawn logic, timer UI, and the main arena scene.

## Unity Versions

- Original project version: Unity 2019.1.8f1.
- Local modern target version: Unity 6000.4.10f1, found in Unity Hub at `C:\Program Files\Unity\Hub\Editor\6000.4.10f1`.
- `ProjectSettings/ProjectVersion.txt` is intentionally left at 2019.1.8f1 until the project is opened and migrated by Unity. Let Unity write the serialization and settings upgrade in the Editor so scene, prefab, and package migrations are visible as a separate reviewable change.

## Upgrade Preparation Completed

- Added a Unity `.gitignore` so generated folders and IDE files are ignored.
- Preserved `Assets/`, `Packages/`, `ProjectSettings/`, scenes, prefabs, and `.meta` files.
- Confirmed `Assets/Scenes/SampleScene.unity` remains the enabled build scene.
- Refactored observer classes so `Observer` no longer inherits from `MonoBehaviour` while still preserving the existing damage flash and timer observer flow.
- Refactored the plain gameplay data/logic classes `Player`, `EnemyBot`, and `NormalAttack` so they are no longer constructed as `MonoBehaviour` instances.
- Added respawn guards to player and enemy controllers so each death starts only one respawn coroutine.
- Preserved respawn behavior: deactivate object, wait, reset position, health, rotation, and reactivate object.
- Re-enabled the enemy `NavMeshAgent` after respawn.
- Reset `Time.timeScale` when the `GameController` wakes up so previous play sessions do not leave the project paused.
- Updated timer logic to use match elapsed time and show the existing `TimeUpPannel` object if it is present in the scene.
- Added defensive null checks and clearer error messages for key scene references.

## How To Open

1. Open Unity Hub.
2. Add this project folder if it is not already listed.
3. Open it first with Unity 6000.4.10f1.
4. Let Unity import and upgrade assets, packages, scenes, prefabs, and settings.
5. Save the project from Unity after confirming the scene opens cleanly.

## How To Test SampleScene

1. Open `Assets/Scenes/SampleScene.unity`.
2. Confirm the scene contains a `GameController`, Player, Enemy, left and right joystick UI, `DamageImage`, `TimerText`, and `TimeUpPannel`.
3. Press Play.
4. Verify the player can move with the left joystick and aim/shoot with the right joystick.
5. Verify enemy movement still uses the baked NavMesh and follows the player.
6. Verify player damage flashes the damage image.
7. Verify player and enemy deaths trigger only one respawn each.
8. Verify the timer reaches `01:00`, shows `TimeUpPannel`, and pauses the match.
9. Stop Play Mode, then press Play again and confirm time is no longer stuck at `0`.

## Known Upgrade Risks

- The project uses legacy Unity UI `Text` and `Image` components. They should still work, but TextMeshPro migration can be considered later.
- Input is based on the legacy Input Manager and the bundled Virtual Joystick Pack. Keep old Input Manager support enabled when opening in a modern Unity version.
- The enemy relies on a baked NavMesh asset at `Assets/Scenes/SampleScene/NavMesh.asset`. Re-bake the NavMesh in Unity 6000 if the enemy cannot navigate.
- `Packages/manifest.json` contains old packages such as Ads 2.0.8, Analytics 3.3.2, Purchasing 2.0.6, Multiplayer HLAPI 1.0.2, Package Manager UI 2.1.2, Timeline 1.0.0, and XR Legacy Input Helpers 2.0.2. Gameplay scripts do not reference these packages directly. If Unity 6000 reports package resolution errors, remove or upgrade unused service packages through Package Manager as a separate reviewable change.
- Scene and prefab serialization has not been force-upgraded outside Unity. Unity should perform that migration on first open.

## Recommended Next Tasks

- Open the project in Unity 6000.4.10f1 and commit Unity-generated scene, prefab, package lock, and project setting migrations separately.
- Remove unused packages after Unity Package Manager confirms they are not required.
- Add PlayMode tests for respawn and timer behavior.
- Consider moving gameplay model classes into a clearer non-MonoBehaviour folder after the upgrade is stable.
- Review mobile build settings and Android player settings after Unity upgrades the project.
