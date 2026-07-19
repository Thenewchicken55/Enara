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

## The八大 systems (and where they live)

| System | Folder | Purpose |
|---|---|---|
| Core | `Scripts/Runtime/Core/` | GameManager singleton, GameStateMachine, EventBus, ServiceLocator, GameSettings SO, ChapterDefinition SO |
| SceneFlow | `Scripts/Runtime/SceneFlow/` | ChapterDirector (ordered chapter list), SceneLoader (async load with fade) |
| Player | `Scripts/Runtime/Player/` | First-person controller with limping; mouse-look |
| Interaction | `Scripts/Runtime/Interaction/` | IInteractable interface + camera-raycast picker + prompt UI |
| QTE | `Scripts/Runtime/QTE/` | Jesus Prayer QTE loop; heals player after N successes |
| Dialogue | `Scripts/Runtime/Dialogue/` | ScriptableObject graph + runner that plays it |
| Choice | `Scripts/Runtime/Choice/` | IChoiceView + ChoicePresenter that spawns buttons |
| Audio | `Scripts/Runtime/Audio/` | AudioManager (music / SFX / VO through AudioMixer) |
| UI | `Scripts/Runtime/UI/` | Subtitles, Fader, HUD |
| Save | `Scripts/Runtime/Save/` | JSON save with flags + integer stats |
| Input | `Scripts/Runtime/Input/` | InputReader wrapping PlayerControls.inputactions |

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

## Save system

Single slot, JSON, stored at `Application.persistentDataPath/Enara/save.json`. Tracks:

- `currentChapter` - last chapter the player reached
- `flags` - booleans for branching choices (`took_easy_path`, `talked_to_idol`, etc.)
- `stats` - integer counters (QTEs succeeded, etc.)

`SaveSystem.Instance` is a singleton that loads on Awake and saves on every mutation.
Use `SetFlag` from a dialogue choice's `FlagToSet` field to record branching decisions.

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
