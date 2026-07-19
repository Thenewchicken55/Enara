# Roadmap

What's done, what's next, and the order to do it in. Update this as you go.

Legend: [x] done - [ ] todo - [~] partially done / needs assets

## Milestone 0: Project foundation [x]

- [x] Folder structure under `Assets/Enara/`
- [x] Assembly definitions (Runtime + Editor)
- [x] AGENTS.md with build/lint conventions
- [x] Input action asset (`PlayerControls.inputactions`)
- [x] Documentation (this, ARCHITECTURE, SETUP, SCENE_GUIDE, ASSETS, LEARNING)

## Milestone 1: Core systems scaffold [x]

- [x] GameManager + GameStateMachine
- [x] EventBus + ServiceLocator
- [x] GameSettings ScriptableObject
- [x] ChapterDefinition ScriptableObject + ChapterDirector + SceneLoader
- [x] PlayerController (first-person, CharacterController, limp flag)
- [x] FirstPersonLook (yaw on body, pitch on camera)
- [x] Interaction system (IInteractable, Interactor, prompt UI)
- [x] QTE system (Jesus Prayer loop, heal after N successes)
- [x] Dialogue system (ScriptableObject graph + runner)
- [x] Choice system (IChoiceView + ChoicePresenter)
- [x] AudioManager (music/sfx/vo via AudioMixer, singleton)
- [x] Subtitles, Fader, HUD
- [x] SaveSystem (JSON, flags + stats)

## Milestone 1.5: Expanded gameplay systems [x]

- [x] Cutscene system (CutscenePlayer, CutsceneSignalReceiver, CameraShake, SubtitleSequencePlayer)
- [x] NPC AI (NPCController, WaypointPatrol, LookAtPlayer, CompanionController for Old Man)
- [x] World triggers (TriggerZone, PathBranchRouter, WitnessTrigger, MoodLightingController, FootstepController)
- [x] Story branching (MoralityTracker, EndingDirector, PlayerAppearance/sigil, ScriptureReciter, HolyLightController, WakeUpSequence, ChapterCheckpoint, MiracleEvent)
- [x] Menus (MainMenu, PauseMenu, SettingsMenu, LoadingScreen, CreditsRoll, DecisionSummaryUI)
- [x] Path-specific (GlideController for tower descent, TowerProgressionController, LocalizationProvider)
- [x] Debug cheats (F1-F12 dev keys)
- [x] Editor tools (BuildTools for CLI builds, ChapterValidator, ReadOnlyAttribute)

## Milestone 2: First playable vertical slice [ ] (USER - in Unity Editor)

These tasks require the Unity Editor - they cannot be done by writing text files alone.
Follow `docs/SCENE_GUIDE.md` for step-by-step instructions.

- [ ] Open the project in Unity Hub, let it compile, fix any errors that appear
- [ ] Create the `Boot.unity` scene with GameManager / SaveSystem / AudioManager / Fader /
      ChapterDirector / SceneLoader / InputReader
- [ ] Create the `GameSettings.asset` under `Assets/Enara/Settings/` (Assets > Create > Enara > GameSettings)
- [ ] Create the `AudioMixer.mixer` under `Assets/Enara/Settings/` with MasterVolume /
      MusicVolume / SfxVolume exposed parameters
- [ ] Create at least one `ChapterDefinition` asset (e.g. `Crash_Limp`) and wire it into
      `ChapterDirector.chapters`
- [ ] Create a `Forest_Limp.unity` scene with a Player capsule + floor + a few obstacles
- [ ] Add the `QuickTimeEventSystem` component and wire the UI references
- [ ] Press Play - the QTE loop should fire periodically and the player should be able to move
      (limping) with WASD + mouse look

## Milestone 3: Driving intro cutscene [ ] (USER - needs Timeline)

- [ ] Create `Intro_Drive.unity` scene
- [ ] Add a Timeline with a car animating along a spline
- [ ] Use Cinemachine (install via Package Manager if not present) for the camera
- [ ] Trigger the crash (Timeline signal -> ChapterDirector.Advance())

## Milestone 4: Path 1 - The Old Man [ ]

- [ ] Model / find assets for the idol, old man, Virgin Mary (use placeholder cubes for now)
- [ ] Author the idol dialogue graph (persuasion -> choices -> take easy path OR refuse)
- [ ] Wire the "take easy path" flag into a ChapterDirector branch
- [ ] Author the old man encounter
- [ ] Author the Virgin Mary "Wake up" scene (cutscene that reveals the noose)

## Milestone 5: Path 2 - Tower of Babel [ ]

- [ ] Town scene (Terrain + simple buildings)
- [ ] Slavery / dirt-hauling NPC behavior (just animation loops are fine)
- [ ] Multi-language voice acting - record or use TTS for placeholder

## Milestone 6: Path 3 - Monastery [ ]

- [ ] Monastery scene
- [ ] Empty church with the icon of Christ (interactable)
- [ ] Recite Psalm 50 subtitle sequence on interaction

## Milestone 7: Ending [ ]

- [ ] Modern village / hospital scene
- [ ] Denial sequence (subtitle / VO)
- [ ] Resolution / credits

## Milestone 8: Polish [ ]

- [ ] Replace placeholder meshes with real art
- [ ] Bake lighting
- [ ] Mix audio (AudioMixer groups)
- [ ] Localization framework (if needed for Path 2)
- [ ] Settings menu (volume sliders, sensitivity)
- [ ] Build a standalone (Build Profiles)

## What I (opencode) cannot do for you

These require the Unity Editor or human creative work - I can write code but not produce
binary art assets, scene files (safely), or voice recordings:

- Open the project and confirm scripts compile (you must open Unity)
- Create `.unity` scene files (complex YAML; risky to author by hand)
- Create prefabs (`.prefab`)
- Assign scripts to GameObjects / wire Inspector references
- Create the `AudioMixer.mixer` asset (binary / specific YAML)
- Author Timeline tracks
- Set up NavMesh / lighting bake
- Make 3D models, materials, textures, animations, voice recordings

For each of these, see `docs/SCENE_GUIDE.md` for click-by-click instructions.
