# Architecture

This document explains how the Enara codebase fits together. If you are new to Unity or
to this project, read this together with `README.md` (the game design) and `SETUP.md`
(how to open the project).

## High-level diagram

```
                +-----------------------------+
                |         Boot scene          |
                |  GameManager (persistent)   |
                |  SaveSystem (persistent)    |
                |  AudioManager (persistent)  |
                |  Fader (persistent)         |
                |  ChapterDirector            |
                |  SceneLoader                |
                +-----------------------------+
                              |
                              | loads chapter scene
                              v
   +-------------------------------------------------------+
   |            Chapter scene (e.g. Forest_Limp)           |
   |                                                       |
   |   PlayerController  +-- FirstPersonLook               |
   |        |                                             |
   |        v                                             |
   |   InputReader (wraps PlayerControls.inputactions)     |
   |                                                       |
   |   Interactor  --raycast-->  IInteractable (Idol,...)  |
   |                                                       |
   |   QuickTimeEventSystem  ----->  QteUI                 |
   |        |                                              |
   |        v                                              |
   |   Subtitles (Jesus Prayer word per success)           |
   |                                                       |
   |   DialogueRunner  ----->  ChoicePresenter             |
   |        |                    (spawn buttons)            |
   |        v                                              |
   |   DialogueGraph (ScriptableObject)                    |
   +-------------------------------------------------------+
```

## The systems (and where they live)

| System | Folder | Purpose |
|---|---|---|
| Core | `Scripts/Runtime/Core/` | GameManager singleton, GameStateMachine, EventBus, ServiceLocator, GameSettings SO, ChapterDefinition SO |
| SceneFlow | `Scripts/Runtime/SceneFlow/` | ChapterDirector (ordered chapter list), SceneLoader (async load with fade) |
| Player | `Scripts/Runtime/Player/` | First-person controller with limping; mouse-look |
| Interaction | `Scripts/Runtime/Interaction/` | IInteractable interface + camera-raycast picker + prompt UI |
| QTE | `Scripts/Runtime/QTE/` | Jesus Prayer QTE loop; heals player after N successes |
| Dialogue | `Scripts/Runtime/Dialogue/` | ScriptableObject graph + runner that plays it |
| Choice | `Scripts/Runtime/Choice/` | IChoiceView + ChoicePresenter that spawns buttons |
| Audio | `Scripts/Runtime/Audio/` | AudioManager singleton (music / SFX / VO through AudioMixer) |
| UI | `Scripts/Runtime/UI/` | Subtitles, Fader, HUD |
| Save | `Scripts/Runtime/Save/` | JSON save with flags + integer stats |
| Input | `Scripts/Runtime/Input/` | InputReader wrapping PlayerControls.inputactions |
| Cutscene | `Scripts/Runtime/Cutscene/` | CutscenePlayer (Timeline wrapper), CutsceneSignalReceiver, CameraShake, SubtitleSequencePlayer |
| NPC | `Scripts/Runtime/NPC/` | NPCController base, WaypointPatrol (Babel slaves), LookAtPlayer (idol head), CompanionController (Old Man) |
| World | `Scripts/Runtime/World/` | TriggerZone, PathBranchRouter, WitnessTrigger, MoodLightingController, FootstepController, GlideController, TowerProgressionController, LocalizationProvider |
| Story | `Scripts/Runtime/Story/` | MoralityTracker, EndingDirector, PlayerAppearance (sigil), ScriptureReciter SO, HolyLightController, WakeUpSequence, ChapterCheckpoint, MiracleEvent SO |
| Menu | `Scripts/Runtime/Menu/` | MainMenu, PauseMenu, SettingsMenu, LoadingScreen, CreditsRoll, DecisionSummaryUI |
| Utility | `Scripts/Runtime/Utility/` | DebugCheats (F1-F12 dev keys) |
| Editor | `Scripts/Editor/` | BuildTools (CLI build), ChapterValidator, ReadOnlyAttribute |

## Game state machine

The whole game has one `GameState` enum. Transitions go through `GameManager.State.TransitionTo(...)`:

```
Boot ----> Cutscene ----> Exploration <----> Qte
                            ^
                            v
                          Dialogue
                            ^
                            v
                          Menu / Paused
                            ^
                            v
                          Ending
```

Other systems subscribe to `OnStateChanged` to enable/disable themselves:

- `PlayerController` is enabled only in Exploration / Qte.
- `QuickTimeEventSystem` fires only in Exploration.
- `DialogueRunner` puts the state in Dialogue while a conversation is playing.

## Event bus

Cross-system signals use `EventBus` (no references held after unsubscribe). Built-in events:

- `QteSucceededEvent` / `QteFailedEvent` (with QTE id)
- `ChapterStartedEvent` (with chapter id)
- `ChoiceMadeEvent` (with choice id)
- `PlayerInputEnabledEvent`

To add a new event: add a `readonly struct` implementing `IGameEvent` near the bottom of
`EventBus.cs`, then `EventBus.Publish(new MyEvent(...))` and `EventBus.Subscribe<MyEvent>(...)`.

## Chapter flow

A `ChapterDefinition` is a ScriptableObject with an id, scene path, and designer notes.
`ChapterDirector` (in Boot scene) holds an ordered list of these. Calling `StartGame()` loads
chapter 0; calling `Advance()` loads the next; `GoToChapter(id)` jumps.

The list mirrors the README beats:

1. `Intro_Drive` - driving cutscene (Timeline)
2. `Crash_Limp` - player wakes up injured; QTE loop begins
3. `Forest_Paths` - the three-path junction
4. `Path1_OldMan` / `Path2_Babel` / `Path3_Monastery` - the three branches
5. `Ending` - village / hospital; player denies what happened

## Assembly definitions

Two asmdefs:

- `Enara.Runtime` (`Assets/Enara/Scripts/Runtime/`) - all gameplay code, namespace `Enara`.
- `Enara.Editor` (`Assets/Enara/Scripts/Editor/`) - editor-only tools, namespace `Enara.Editor`,
  references `Enara.Runtime`.

Anything outside these folders falls into Unity's default `Assembly-CSharp` and loses the
clean separation. Keep new code inside the asmdefs.

## Input

`PlayerControls.inputactions` lives in `Assets/Enara/Resources/` so it can be loaded with
`Resources.Load` if it's not assigned in the Inspector. It defines:

- **Player** map: Move (WASD/arrows), Look (mouse delta), Interact (E), Advance (Space/mouse),
  Pause (Esc)
- **UI** map: Navigate, Submit, Cancel (for menus)

Two control schemes: KeyboardMouse, Gamepad.

## Where to add new features

| Want to... | Edit... |
|---|---|
| Add a new chapter | Create a `ChapterDefinition` SO under `Assets/Enara/Data/Chapters/`, drag into `ChapterDirector.chapters` |
| Add a dialogue | Create a `DialogueGraph` SO under `Assets/Enara/Data/Dialogue/`, fill with `DialogueNode`s |
| Add an interactable | Implement `IInteractable` on a component, place on a GameObject with a collider |
| Add a new QTE pattern | Edit `QuickTimeEventSystem.kPrayerWords` and the `cycle` array |
| Tweak timing / speeds | Edit the `GameSettings` SO at `Assets/Enara/Settings/GameSettings.asset` |
| Add a new game state | Add to `GameState` enum, handle in `GameStateMachine` and any subscriber |
| Add a new event | Add a `readonly struct : IGameEvent` in `EventBus.cs` |
| Add a cutscene | Author a Timeline, drop a `CutscenePlayer` on a GameObject, call `Play()` from a TriggerZone |
| Add a path-branch choice | Set `FlagToSet` on a `DialogueChoice`, branch in `PathBranchRouter` or `EndingDirector` |
| Add an NPC | Extend `NPCController`, add `WaypointPatrol` + `LookAtPlayer` as needed |
| Add a companion | Use `CompanionController` (already a NavMeshAgent follower) |
| Add a miracle | Create a `MiracleEvent` SO, call `Trigger(transform)` from a Timeline signal or TriggerZone |
| Add a checkpoint | Place a `ChapterCheckpoint` trigger collider |
| Add a localized string | Create a `LocalizationProvider` SO per language, call `Get(key)` |
| Add a build step | Edit `Enara.Editor.BuildTools.BuildStandalone` |

## Cutscene flow

Cutscenes use Unity Timeline + a `CutscenePlayer` wrapper. The wrapper:

1. Puts `GameState` into `Cutscene` (locks player input via `PlayerController.HandleStateChanged`).
2. Optionally fades out via `UI.Fader`.
3. Plays the `PlayableDirector`.
4. Waits for the timeline to finish.
5. Fades back in.
6. Returns `GameState` to `Exploration` (or whatever it was).
7. Fires `OnFinished` for chaining.

For Timeline signals (e.g. crash impact frame), use a `CutsceneSignalReceiver` on the same
GameObject as the PlayableDirector. Wire each signal asset to a UnityEvent in the inspector
(e.g. `CameraShake.Shake()` / `AudioManager.PlaySfx(crashClip)`).

## Story / branching flow

```
                  DialogueChoice.FlagToSet
                       |
                       v
            SaveSystem.SetFlag("took_easy_path")
                       |
                       v
       +---------------+---------------+
       |               |               |
   EndingDirector  PathBranchRouter  PlayerAppearance
   (picks ending) (chooses path)   (applies sigil)
       |
       v
   MoralityTracker (nearnessToGod, temptedAway)
       |
       v
   DecisionSummaryUI (shows at end)
```

The `MoralityTracker` listens for `GameState.Ending` and persists its two floats as integer
stats in the save file. `EndingDirector.ResolveEnding()` then picks:

- `BadEasyPath` if `took_easy_path` flag is set,
- `Resolution` if `nearness >= 70`,
- `Denial` otherwise.

## NPC AI

- **`NPCController`** - base class. Holds id, display name, animator. `Pause()` / `Resume()`
  for cutscene control.
- **`WaypointPatrol`** - NavMeshAgent looped patrol. Used for Babel slaves hauling dirt.
  Bake a NavMesh in any scene that uses them.
- **`LookAtPlayer`** - smoothly rotates a bone (or whole object) to face the player. Used
  for the idol statue turning its head, ambient NPCs reacting.
- **`CompanionController`** - NavMeshAgent follower for the Old Man. `Dismiss(target)` makes
  him walk away (Mary's "Leave him alone. Go, now."). `Rejoin()` brings him back.

## Save system

Single slot, JSON, stored at `Application.persistentDataPath/Enara/save.json`. Tracks:

- `currentChapter` - last chapter the player reached
- `flags` - booleans for branching choices (`took_easy_path`, `talked_to_idol`, `has_sigil`,
  `miracle_<id>`, `visited_<checkpointId>`, etc.)
- `stats` - integer counters (QTEs succeeded, morality nearness, morality temptation)

`SaveSystem.Instance` is a singleton that loads on Awake and saves on every mutation.
Use `SetFlag` from a dialogue choice's `FlagToSet` field to record branching decisions.
