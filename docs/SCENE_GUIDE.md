# Scene Guide

Click-by-click instructions for wiring up each Unity scene. This is the part opencode
cannot do for you - every step below requires the Unity Editor.

Conventions:
- Scenes live in `Assets/Enara/Scenes/`.
- Prefabs (when you make them) live in `Assets/Enara/Prefabs/`.
- All caps = a menu path, e.g. `GameObject > Camera`.

---

## Scene 0: Boot.unity

This is the entry point. It loads the persistent singletons and starts the game.

1. **File > New Scene > Empty (URP)**. Save as `Assets/Enara/Scenes/Boot.unity`.
2. Create an empty GameObject named `GameRoot`. Add these components:
   - **GameManager** (`Assets/Enara/Scripts/Runtime/Core/GameManager.cs`)
   - **SaveSystem** (`Save/SaveSystem.cs`)
   - **AudioManager** (`Audio/AudioManager.cs`) - add three `AudioSource` children for
     Music / SFX / VO, drag them into the AudioManager's `musicSource` / `sfxSource` / `voSource`.
     Drag the `AudioMixer` (from step 4 of SETUP) into `mixer`.
   - **Fader** (`UI/Fader.cs`) - add an `Image` component, set color to black with alpha 0.
     Make it cover the screen (Rect Transform stretch both axes, width/height 0).
   - **SceneLoader** (`SceneFlow/SceneLoader.cs`)
   - **ChapterDirector** (`SceneFlow/ChapterDirector.cs`) - drag the SceneLoader into its
     `sceneLoader` slot. Drag your `ChapterDefinition` assets into the `chapters` list.
   - **InputReader** (`Input/InputReader.cs`) - the `actionsAsset` auto-loads from Resources;
     you can also drag `Assets/Enara/Resources/PlayerControls.inputactions` into it.
3. Add a `Canvas` (Game Object > UI > Canvas). Set the Canvas Scaler to "Scale With Screen Size"
   with reference resolution 1920x1080. This is the parent for Fader / Subtitles / QteUI / etc.
4. Move the Fader's Image to be a child of this Canvas if it isn't already.
5. Open `File > Build Profiles` (formerly Build Settings) and **Add Open Scene** so Boot is at
   index 0. This makes it the first scene loaded when the game runs.

**Verify**: Press Play. No errors in console. The Fader should briefly fade in/out.

---

## Scene 1: Intro_Drive.unity

The driving cutscene. This is mostly Timeline - the player has no input.

1. **File > New Scene > Empty (URP)**. Save as `Intro_Drive.unity`.
2. Add the car model (placeholder: a Cube scaled to look like a car) at the start of a road.
3. Add a `PlayableDirector` GameObject. Create a `PlayableAsset` (Timeline) and:
   - Animate the car along the road using an Animation Track.
   - (Optional) Use Cinemachine for a tracking camera. Install Cinemachine via Package Manager
     if not present (`com.unity.cinemachine`).
   - Add a Signal Track that, at the end (the crash), calls
     `ChapterDirector.Advance()` to load the next scene.
4. Set `GameManager.State` to `Cutscene` in Awake (via a small script on the car) so the
   player input is locked.

---

## Scene 2: Forest_Limp.unity

The crash aftermath. **This is the first gameplay scene.** Core of the README QTE loop.

1. **File > New Scene > Empty (URP)**. Save as `Forest_Limp.unity`.
2. Add a floor plane (`GameObject > 3D Object > Plane`). Make sure it has a Collider.
3. Add the player:
   - Create an empty GameObject `Player`.
   - Add **CharacterController**.
   - Add **PlayerController** (`Player/PlayerController.cs`).
   - Add a child Camera at eye height (~1.6 on Y). Drag it into `FirstPersonLook.cameraTransform`.
   - Add **FirstPersonLook** to the player body. Drag the Camera into `cameraTransform`,
     the body itself into `bodyTransform`.
   - Add **Interactor** (`Interaction/Interactor.cs`). Drag the Camera into `cam`.
4. Add the QTE system:
   - Add an empty GameObject `QTESystem`.
   - Add **QuickTimeEventSystem** (`QTE/QuickTimeEventSystem.cs`).
   - Drag the `Player` into `player`.
   - Drag the Subtitles UI into `subtitles` (created below).
   - Drag the QteUI into `ui` (created below).
5. Add UI (children of the Boot Canvas, or a new local Canvas in this scene):
   - **Subtitles**: a Panel + a TextMeshProUGUI child. Add the `Subtitles` component to the Panel,
     drag the TMP into `label`.
   - **InteractionPromptUI**: another Panel + TMP. Add the `InteractionPromptUI` component.
   - **QteUI**: a Panel + a TMP for the word + a TMP for the key hint + an Image (filled) for
     the timer bar. Add the `QteUI` component, wire them all in.
6. Make sure an `InputReader` is in the scene (drag the prefab from Boot, or add the component
   again here - whichever is easier).
7. Set the player's `Limping = true` (it's true by default).

**Verify**: Press Play. The player walks slowly with WASD + mouse look. Every 4-8 seconds a QTE
fires. Press the right key (shown on the QteUI) within the window to recite the next word of the
Jesus Prayer. After 8 successes the limp goes away.

---

## Scene 3: Forest_Paths.unity

The three-path junction.

1. Create a scene with three diverging paths (cubes / terrain).
2. Add trigger colliders at the entrance of each path. Implement a small script:

```csharp
using UnityEngine;
using Enara.SceneFlow;
using Enara.Core;

public sealed class PathTrigger : MonoBehaviour
{
    [SerializeField] private ChapterId targetChapter;
    [SerializeField] private ChapterDirector director;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        director.GoToChapter(targetChapter);
    }
}
```

3. Place one `PathTrigger` at each path's entrance, pointing at `Path1_OldMan`,
   `Path2_Babel`, `Path3_Monastery`.

---

## Path 1: Path1_OldMan.unity

### Angel encounter (entrance)
1. Add an empty GameObject `AngelLight` with a `HolyLightController` component.
   - Wire `root` to itself.
   - Add a `ParticleSystem` child for the light beam (optional).
   - Add an `AudioSource` for the ambient hum.
   - Wire `appearSfx` / `disappearSfx` if you have them.
2. Add a `TriggerZone` on a collider at the path entrance. In its `onEnter` event:
   - Call `AngelLight.Appear()`.
   - (Optional) Start a `SubtitleSequencePlayer` with the angel's warning lines.
3. Add a second `TriggerZone` further down the path; its `onEnter` calls `AngelLight.Disappear()`.

### Idol statue
1. Add the Idol GameObject (placeholder: a tall gold cylinder). Add a collider sized to its
   bounds. Implement `IInteractable` on a small component:

```csharp
using UnityEngine;
using Enara.Interaction;
using Enara.Dialogue;

public sealed class IdolInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueGraph idolDialogue;
    [SerializeField] private DialogueRunner runner;

    public string PromptText => "Press E to address the idol";
    public bool CanInteract => true;

    public void Interact(GameObject interactor)
    {
        runner.StartDialogue(idolDialogue);
    }
}
```

2. Add a `LookAtPlayer` component to the idol. Drag its head bone (or the whole idol) into
   `bone`. Set `range` to ~6 m. Now the idol turns its head when the player approaches.
3. Create the `idolDialogue` asset: right-click `Assets/Enara/Data/Dialogue/`,
   **Create > Enara > Dialogue Graph**. Open it and add nodes:
   - `idol_intro`: "I can save you, if you will only bow."
   - choices:
     - `bow` -> label "Bow to the idol" -> target `idol_easy_path`, FlagToSet `took_easy_path`
     - `refuse` -> label "Refuse, return to prayer" -> target `idol_refused`
   - `idol_easy_path`: leads to the bad ending
   - `idol_refused`: ends the conversation; player continues
4. Drop the `DialogueGraph` and the `DialogueRunner` into the IdolInteractable.
5. On the `bow` choice's flag, also wire `PlayerAppearance.ApplySigil()` from a UnityEvent
   so the sigil tattoo is applied.

### Old Man walk
1. Add the Old Man GameObject. Add:
   - `NavMeshAgent` (set speed to ~1.4)
   - `CompanionController` (drag the player into `playerTransform`, set follow distance ~2.5)
   - `Collider` + `Rigidbody` (kinematic) so the player can't walk through him.
2. Bake a NavMesh (`Window > AI > Navigation > Bake`) over the walkable area.
3. Add a `TriggerZone` at the start of the walk path. Its `onEnter`:
   - Enables the Old Man GameObject (he was hidden).
   - Calls `CompanionController.Rejoin()` if he was dismissed.
4. Add a `MoodLightingController` to the scene. Set `startAmbient` bright, `endAmbient` near
   black, `durationSeconds` to ~120 (the length of the walk). Wire a TriggerZone at the walk's
   start to `MoodLightingController.BeginTransition()`.

### Noose wake-up
1. Place a noose prop in a hidden area of the scene (or its own scene).
2. Add a `WakeUpSequence` component to an empty GameObject. Wire:
   - `cutscenePlayer` (optional Timeline cutscene)
   - `marySubtitles` - a `SubtitleSequencePlayer` with these lines:
     - Mary: "Leave him alone. Go, now." (2 s)
     - Mary: "Do you know where you are? Wake up" (3 s)
     - Mary: "Look around you. Wake up" (3 s)
     - Mary: "Lord have mercy, Wake Up!" (3 s)
   - `mary` - the HolyLightController on the Mary GameObject.
   - `oldMan` - the CompanionController; `oldManLeaveTarget` - where he walks off to.
   - `nooseCameraPosition` - a Transform near the noose where the camera cuts to.
   - `playerCamera` - the player's Camera.
3. Add a `TriggerZone` at the end of the walk. Its `onEnter` calls `WakeUpSequence.Trigger()`.

---

## Path 2: Path2_Babel.unity

1. Terrain with a town on a mountain.
2. NPCs (placeholder capsules) - add a `NavMeshAgent` + `WaypointPatrol` to each slave.
   - Add ~4 waypoint transforms along the dirt-hauling path.
   - Drag them into the slave's `waypoints` list.
   - Bake a NavMesh.
3. Add `LookAtPlayer` to NPCs that should react (the town leader, the thieves).
4. Add a `TowerProgressionController` with each stage as a GameObject child of the tower.
   As the player climbs, TriggerZones call `ShowStage(1)`, `ShowStage(2)`, etc. to reveal
   the tower getting taller.
5. The leader's dialogue at the top leads to the Tower-of-Babel punishment cutscene. Use a
   `CutscenePlayer` + `SubtitleSequencePlayer` with multi-language VO.
6. For the multi-language VO beat, create a `LocalizationProvider` SO per language
   (`Assets/Enara/Data/Localization/<lang>.asset`).
7. At the top, give the player a glide ability:
   - Add a `GlideController` to the player.
   - Wire a `TriggerZone` that calls `GlideController.Unlock()` when they pick up the
     hang-glider / robe.

---

## Path 3: Path3_Monastery.unity

1. Monastery exterior + interior. Loud angelic chants (loop an audio clip via the
   `AudioManager.PlayMusic(...)`).
2. The church interior is empty except for an icon of Christ.
3. Add an `IInteractable` on the icon. On interaction:
   - Create a `ScriptureReciter` SO (`Assets/Enara/Data/Scripture/Psalm50.asset`) with the
     verses of Psalm 50.
   - The interactable calls `scriptureReciter.Recite(subtitleSequencePlayer)`.
4. Add `MiracleEvent` SOs (`Assets/Enara/Data/Miracles/<name>.asset`) for the miracles the
   README mentions. Wire a TriggerZone to call `miracleEvent.Trigger(playerTransform)`.
5. (Optional) Add a `WitnessTrigger` on a specific object the player must look at before
   something happens - wire its `onWitnessed` event.

---

## Ending: Ending.unity

1. Modern village / hospital scene.
2. Add an `EndingDirector` component. Wire:
   - `morality` - the MoralityTracker from the persistent Boot scene.
   - `badEasyPathCutscene` / `denialCutscene` / `resolutionCutscene` - three CutscenePlayers
     for each ending.
3. Add a `DecisionSummaryUI` somewhere on the ending Canvas. Author `knownFlags`:
   - `took_easy_path` -> "You bowed to the idol for an easy rescue."
   - `path1_chosen` -> "You walked the path of the Old Man."
   - `path2_chosen` -> "You climbed the Tower of Babel."
   - `path3_chosen` -> "You entered the monastery."
   - `has_sigil` -> "You bear the sigil of the idol."
4. At the end of the chosen ending cutscene, fire a Timeline signal that calls
   `CreditsRoll.Instance.Play()`.

---

## Common gotchas

- **CharacterController + Animator**: if your Animator applies root motion, you'll fight the
  CharacterController. Either disable root motion or move via the Animator.
- **UI events vs game input**: when in `GameState.Dialogue`, the PlayerController is disabled
  so it stops reading input - but the InputReader is still active, so choices still fire.
- **DontDestroyOnLoad duplication**: GameManager / SaveSystem / AudioManager / Fader live in
  Boot and persist. If you accidentally also put them in another scene, you'll get duplicates
  - the singletons self-destruct if a duplicate is detected, but only after Awake.
- **Scene ordering in Build Profiles**: only `Boot.unity` needs to be at index 0. Other scenes
  can be added but they don't need to be in the list (ChapterDirector loads them by path).
