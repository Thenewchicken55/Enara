# AGENTS.md

This file gives opencode (and any other AI assistant / contributor) the context it needs to
work correctly on the **Enara** Unity project.

## Project overview

`Enara` is a narrative-driven first/third-person Unity game about a spiritual journey.
The full design lives in `README.md`. Read it before touching gameplay code.

- **Engine:** Unity 6 (`6000.5.2f1`) - URP (Universal Render Pipeline)
- **Input:** Unity New Input System (`com.unity.inputsystem` 1.19+)
- **Language:** C# 9 (Unity 6 default)
- **Repo:** https://github.com/Thenewchicken55/Enara.git

## Folder layout

```
Assets/Enara/
  Scripts/
    Runtime/        <- gameplay code, organized by feature folder
      Core/          <- GameManager, GameStateMachine, EventBus, ServiceLocator, GameSettings, ChapterDefinition
      SceneFlow/     <- ChapterDirector, SceneLoader
      Player/        <- PlayerController, FirstPersonLook
      Interaction/   <- IInteractable, Interactor, InteractionPromptUI
      QTE/           <- QuickTimeEventSystem (Jesus Prayer mechanic)
      Dialogue/      <- DialogueGraph (ScriptableObject), DialogueRunner
      Choice/        <- ChoicePresenter, IChoiceView
      Audio/         <- AudioManager (singleton), MusicPlayer
      UI/            <- Subtitles, Fader (singleton), HUD
      Save/          <- SaveSystem (JSON)
      Input/         <- InputReader (wraps PlayerControls.inputactions)
      Cutscene/      <- CutscenePlayer, CutsceneSignalReceiver, CameraShake, SubtitleSequencePlayer
      NPC/           <- NPCController, CompanionController, WaypointPatrol, LookAtPlayer
      World/         <- TriggerZone, PathBranchRouter, WitnessTrigger, MoodLightingController, FootstepController, GlideController, TowerProgressionController, LocalizationProvider
      Story/         <- MoralityTracker, EndingDirector, PlayerAppearance, ScriptureReciter, HolyLightController, WakeUpSequence, ChapterCheckpoint, MiracleEvent
      Menu/          <- MainMenu, PauseMenu, SettingsMenu, LoadingScreen, CreditsRoll, DecisionSummaryUI
      Utility/       <- DebugCheats (F1-F12 dev keys)
      ScriptableObjects/ <- shared SO definitions (kept for compatibility)
    Editor/          <- editor-only tools / inspectors
      BuildTools, ChapterValidator, ReadOnlyAttribute
  Scenes/            <- .unity scenes (Boot, Intro_Drive, Forest_Limp, ...)
  Prefabs/           <- .prefab files
  Resources/         <- runtime-loaded assets (e.g. InputReader, default settings)
  Art/               <- textures, materials, models
  Audio/             <- music + SFX clips
  Animations/        <- animators + clips
  Data/              <- ScriptableObject instances (dialogue graphs, chapters...)
  Settings/          <- project-local settings (URP assets, AudioMixer, etc.)
docs/                <- human-readable design + setup docs (read these!)
```

## Build / run / lint commands

This is a Unity project - there is no CLI build by default. The standard workflow is:

- **Open the project:** launch Unity Hub, click "Add project from disk", pick this folder.
  First import recompiles all scripts; this can take a few minutes.
- **Run the game:** open `Assets/Enara/Scenes/Boot.unity` and press the Play button.
- **Build a standalone:** `File > Build Profiles > Build` (Unity 6 renamed Build Settings to Build Profiles).

There is no `npm`/`dotnet`/`cargo`-style task runner. For automated / headless builds you can
call Unity from CLI (documented in `docs/SETUP.md`), but for day-to-day work just use the Editor.

## Type-check / compile verification

Unity compiles C# on focus / on save. To verify a script compiles:

1. Save it.
2. Switch to the Unity Editor window - it recompiles automatically.
3. Watch the console (`Ctrl+Shift+C`); red lines = compile errors.

If you add a new C# file outside the Editor, make sure it is inside a folder covered by an
`.asmdef` (currently `Assets/Enara/Scripts/Runtime/` or `Assets/Enara/Scripts/Editor/`).
Files outside any asmdef fall into Unity's default `Assembly-CSharp`, which is fine but
loses the clean separation we want.

If you want to compile from CLI without opening Unity, see `docs/SETUP.md` for the
`unity -batchmode -nographics` invocation. It is slow; usually not worth it.

## Conventions

- **Namespace:** `Enara` for runtime code, `Enara.Editor` for editor code. Match the folder name.
- **One class per file.** File name matches the class name.
- **No `public` fields on MonoBehaviours** - use `[SerializeField] private`.
- **ScriptableObjects** define data; **MonoBehaviours** define behaviour. Don't mix.
- **Comments:** the codebase uses XML `///` doc comments on public APIs. Don't add inline
  `//` comments unless something is genuinely surprising.
- **Commit style:** short imperative subject (`Add QTE failure state`), no Co-authored-by footer.
- **Never commit** the `Library/`, `Temp/`, `Obj/`, `Build/`, `Logs/` folders (already in .gitignore).
- **Never commit** large binary assets (FBX, WAV, PNG > ~256 KB) without checking with the user first.

## Important files to read first

- `README.md` - the game design
- `docs/ARCHITECTURE.md` - how the code fits together
- `docs/ROADMAP.md` - what's done and what's still TODO, in order
- `docs/SETUP.md` - first-time setup walkthrough
- `docs/SCENE_GUIDE.md` - how to wire up each Unity scene
