# CLAUDE.md — Project Context for AI Assistants

## Project

Mobile-first (Android) 3D top-down hole-absorption game (Hole.io / Jelly Hole style).
All custom scripts live in `Assets/DimaD2/Scripts/`. Namespace: `DimaD2`.
No TopDown Engine dependencies in custom scripts.

---

## Scene Wiring (Prototype_Level_01)

### GameManager GameObject

Add a single empty GameObject named `GameManager` to the scene root.

Components:
- `GameManager` — singleton; set **LevelTimer** and **ObjectiveSystem** fields in the Inspector

```
GameManager (GameObject)
  └── GameManager.cs
        ├── Level Timer  → drag LevelTimer GO here
        └── Objective System → drag ObjectiveSystem GO here
```

### LevelTimer GameObject

Separate empty GameObject named `LevelTimer`.

Components:
- `LevelTimer` — set **Duration** (seconds) in the Inspector (default 60)

### ObjectiveSystem GameObject

Separate empty GameObject named `ObjectiveSystem`.

Components:
- `ObjectiveSystem` — populate the **Objectives** list in the Inspector:
  - Each entry: `itemType` string (must match the `Item Type` field on `AbsorbableItem`) + `requiredCount` int
  - Example: `{ itemType: "fruit", requiredCount: 5 }`

### Hole GameObject

The player-controlled hole. All of the following components must be on **the same GameObject**.

Components:
- `HoleController` — movement + absorption trigger handler
- `HoleSizeSystem` — level-up logic; Inspector fields:
  - **Base Threshold** — progress needed to reach level 1 (default 5)
  - **Start Size** — initial `currentSize` / collider radius (default 1)
  - **Size Per Level** — how much `currentSize` grows per level (default 0.5)
- `SphereCollider` — **must be set as Trigger** (`Is Trigger = true`); radius should match **Start Size**

```
Hole (GameObject)
  ├── HoleController.cs
  ├── HoleSizeSystem.cs
  └── SphereCollider  (Is Trigger = true, Radius = 1)
```

`HoleSizeSystem` updates both `HoleController.currentSize` (via `SetSize()`) and `SphereCollider.radius` automatically on level-up. No manual wiring needed between those two.

### AbsorbableItem GameObjects

Every absorb-able object in the scene needs:
- `AbsorbableItem` component
- **Item Size** — float; object is absorbed only when `itemSize <= holeSize`
- **Item Type** — string; must match an entry in `ObjectiveSystem` to count toward objectives (leave blank if not part of any objective)
- A `Collider` (any shape) — does **not** need to be a trigger; the hole's trigger detects it

---

## What Is Still Missing for Phase 1 MVP

### 1. UI

Nothing is connected to the UI layer yet. Needed:
- **Timer display** — read `LevelTimer.TimeRemaining` each frame and show it on a `TextMeshProUGUI` label
- **Size level display** — read `HoleSizeSystem.SizeLevel` (and optionally `Progress` / threshold for a progress bar)
- **Objective panel** — display each `ObjectiveEntry.currentCount / requiredCount`; requires exposing the list as `public` or adding a getter to `ObjectiveSystem`
- **Win screen** — shown when `GameManager.State == GameState.Win`
- **Fail screen** — shown when `GameManager.State == GameState.Fail`
- **Restart button** — calls `GameManager.Instance.RestartLevel()`

### 2. Actual Level Content

The prototype scene has no `AbsorbableItem` objects placed yet. Needed:
- Place objects with `AbsorbableItem` components, varied `itemSize` values, and `itemType` strings
- At minimum one item type matching an `ObjectiveSystem` entry so the win condition is reachable

### 3. Hole Visual Scaling

`HoleSizeSystem` updates `currentSize` and `SphereCollider.radius`, but does **not** scale the hole's visual mesh. Needed:
- Drive `transform.localScale` from `HoleSizeSystem.ApplySize()`, or
- Use a separate visual child GameObject with its own scale driven by `SizeLevel`

### 4. Camera

`TopDownCameraFollow.cs` exists but its integration with the new hole GameObject may need re-checking after scene changes. Confirm the camera target is set to the Hole GameObject.

### 5. Absorb Feedback

No juice on absorb. Nice-to-have for MVP feel:
- Brief scale punch on the hole
- Object shrink-to-center tween before deactivation

### 6. Objective Completion / Time-Up Response

`GameManager` logs Win/Fail to the console but takes no in-game action beyond that (no pause, no UI show). The Win/Fail screens from item 1 above are what make this visible to the player.

### 7. Scene Not in Build Settings

`RestartLevel()` uses `SceneManager.LoadScene(buildIndex)`. The prototype scene must be added to **File → Build Settings → Scenes In Build** or the reload will fail.

---

## Code Map

| File | Purpose |
|---|---|
| `AbsorbableItem.cs` | Data + absorb behaviour per object (`itemSize`, `itemType`) |
| `HoleController.cs` | Movement, trigger detection, calls HoleSizeSystem + ObjectiveSystem |
| `HoleSizeSystem.cs` | Discrete level-up, 2x threshold scaling, updates size + collider |
| `LevelTimer.cs` | Countdown, fires `OnTimeUp` |
| `ObjectiveSystem.cs` | Tracks item-type counts, fires `OnAllObjectivesComplete` |
| `GameManager.cs` | Singleton, Win/Fail state, `RestartLevel()` |
| `TopDownCameraFollow.cs` | Follows hole on XZ plane |
