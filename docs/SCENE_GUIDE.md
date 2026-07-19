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

1. Add the Idol statue GameObject (placeholder: a tall gold cylinder). Add a collider sized to
   its bounds. Implement `IInteractable` on a small component:

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

2. Create the `idolDialogue` asset: right-click `Assets/Enara/Data/Dialogue/`,
   **Create > Enara > Dialogue Graph**. Open it and add nodes:
   - `idol_intro`: "I can save you, if you will only bow."
   - choices:
     - `bow` -> label "Bow to the idol" -> target `idol_easy_path`, FlagToSet `took_easy_path`
     - `refuse` -> label "Refuse, return to prayer" -> target `idol_refused`
   - `idol_easy_path`: leads to the bad ending
   - `idol_refused`: ends the conversation; player continues
3. Drop the `DialogueGraph` and the `DialogueRunner` into the IdolInteractable.
4. Add the Old Man GameObject with another `IInteractable` that triggers the walking sequence.
5. The Virgin Mary "Wake up" beat: at the end of the old-man walk, fire a Timeline cutscene
   with subtitles for Mary's lines.

---

## Path 2: Path2_Babel.unity

1. Terrain with a town on a mountain.
2. NPCs (placeholder capsules) with idle animations.
3. Dirt-hauling: a few NPCs that walk a looped path (use NavMeshAgent + a simple patrol
   script, or just an Animator with a root-motion walk).
4. The leader's dialogue at the top leads to the Tower-of-Babel punishment cutscene.

---

## Path 3: Path3_Monastery.unity

1. Monastery exterior + interior. Loud angelic chants (loop an audio clip via the
   `AudioManager.PlayMusic(...)`).
2. The church interior is empty except for an icon of Christ.
3. `IInteractable` on the icon: on interaction, play the Psalm 50 subtitles
   (create a `DialogueGraph` with one node per verse, or just call
   `Subtitles.Show(verse, duration)` in sequence via a coroutine).

---

## Ending: Ending.unity

1. Modern village / hospital scene.
2. Final denial VO + subtitles.
3. Credits roll (use a scrolling RectTransform or a Timeline).

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
