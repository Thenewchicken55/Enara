# Getting Started Tutorial (for Unity beginners)

This walks you from "I just cloned the repo" to "I can walk around and the QTE fires" — step
by step, with every click written out. Expect to spend **2-4 hours** the first time, mostly
waiting for Unity to import.

If you get stuck, read `docs/LEARNING.md` for curated video tutorials, or just ask opencode
to explain any error message.

---

## Step 0: Install Unity + a code editor

### 0.1 Install Unity Hub

1. Go to <https://unity.com/download>.
2. Download **Unity Hub** for your OS (macOS for you, since you're on darwin).
3. Install it. Open it once. Sign in with a Unity ID (free to create).

### 0.2 Install the exact Unity version this project needs

1. In Unity Hub, go to **Installs** on the left.
2. Click **Install Editor**.
3. You need **Unity 6.0.5** with the exact version string `6000.5.2f1`. If it's in the list,
   pick it. If not:
   - Click **Archive** on the install page (or go to
     <https://unity.com/releases/editor/archive>).
   - Find **Unity 6.0.5** and click the green install button next to `6000.5.2f1`.
4. When picking modules, check:
   - **Documentation** (optional but helpful)
   - **Apple Build Support (Mac, iOS)** — since you're on macOS
   - Skip everything else for now.
5. Click Install. This downloads ~3 GB, so go get coffee.

### 0.3 Install a code editor (if you don't already have one)

Pick one of these. VS Code is the lightest:

- **VS Code** (recommended, free): <https://code.visualstudio.com/>
  - After install, also install the **C# Dev Kit** extension from the marketplace.
- **JetBrains Rider** (paid, but the best Unity experience — free for students)
- **Visual Studio for Mac** (free, but discontinued in 2024 — avoid for new installs)

You don't strictly need a code editor for this tutorial — Unity recompiles on its own. But
you'll want one to read and edit the scripts.

---

## Step 1: Open the project

1. Open Unity Hub.
2. Click **Open > Add project from disk**.
3. Pick the folder you cloned the repo into (e.g. `/Users/eliaa/Code/Enara`).
4. The project appears in the list. Click it to open.
5. **Wait 2-5 minutes.** Unity is reimporting all the packages and compiling all the C#
   scripts for the first time. There's a progress bar at the bottom right.
6. When it's done, you'll see the Unity Editor with the default layout (Hierarchy on left,
   Game / Scene view in the middle, Inspector on right).

### 1.1 Check the console for errors

- Top menu: **Window > General > Console** (or press Cmd+0 on Mac, Ctrl+0 on Windows).
- You should see yellow warnings (fine — Unity generates these for unused fields).
- **If you see red errors, stop and read them.** A red error means a script didn't compile.
  Common causes:
  - Wrong Unity version (must be 6000.5.2f1).
  - Missing a package — open **Window > Package Manager** and verify `Input System`,
    `Universal RP`, `Timeline` are all installed (they should be — they're in `manifest.json`).

If everything is clean (no red), continue.

### 1.2 Set your code editor (optional but recommended)

1. Top menu: **Edit > Project Settings > External Tools**.
2. Under **External Script Editor**, pick VS Code (or Rider).
3. Make sure **"Generate .csproj files for:**" has both **Player** and **Editor** checked.

---

## Step 2: Create the GameSettings asset

The code references a `GameSettings` ScriptableObject but the asset itself doesn't exist yet.
You need to create it once.

1. In the **Project window** (bottom of the screen by default), navigate to
   `Assets/Enara/Settings/`.
2. Right-click in empty space → **Create > Enara > GameSettings**.
3. A new asset appears. Rename it to `GameSettings` (no extension — Unity adds it).
4. Click it once. The **Inspector** on the right shows its fields:
   - QTE timings (window, min/max interval, count until healed)
   - Player speeds (walk, limp, look sensitivity)
   - Audio volumes (master, music, sfx)
5. Leave the defaults for now. You can tweak them later.

### 2.1 Move it to Resources (IMPORTANT — this is the one bit I can't automate)

The code looks for the settings via `Resources.Load<GameSettings>("GameSettings")`. For that
to find it, the asset must be in a folder named exactly `Resources`.

1. In the Project window, right-click `Assets/Enara/Resources/` (it should already exist).
2. If `GameSettings` is still in `Assets/Enara/Settings/`, drag it into `Assets/Enara/Resources/`.

**Verify**: select the asset. Its path shows at the top of the Inspector. It should say
`Assets/Enara/Resources/GameSettings.asset`.

---

## Step 3: Create the AudioMixer

The `AudioManager` routes audio through an AudioMixer with three exposed parameters
(`MasterVolume`, `MusicVolume`, `SfxVolume`). You need to create this once.

1. Right-click `Assets/Enara/Settings/` → **Create > Audio Mixer**.
2. Name it `AudioMixer`.
3. Double-click it to open the AudioMixer window.
4. In the window, you'll see one group called **Master**.
5. Click the **+** button next to **Groups** (top right of the groups list) three times to
   add three child groups. Rename them to **Music**, **SFX**, **VO**.
6. For each of the four groups (Master, Music, SFX, VO):
   - Click the group to select it.
   - In the Inspector (right side), find the **Volume** property under **Attenuation**.
   - Right-click the volume slider → **Expose 'Volume' (of [GroupName]) to script**.
7. Now in the AudioMixer window top-right, click **Exposed Parameters** (a small button with
   a little speaker icon). You'll see four parameters named like `MyExposedParameter`. Rename
   them by double-clicking:
   - The Master group's exposed param → `MasterVolume`
   - The Music group's → `MusicVolume`
   - The SFX group's → `SfxVolume`
   - (VO doesn't need to be exposed — only the three the AudioManager reads.)

**Verify**: the AudioMixer window's Exposed Parameters list shows `MasterVolume`, `MusicVolume`,
`SfxVolume`.

---

## Step 4: Create the Boot scene

This is the entry scene. It loads the persistent singletons and starts the game.

### 4.1 Create the scene file

1. Top menu: **File > New Scene**.
2. Pick **Basic (URP)** template. Click **Create**.
3. Delete the default Main Camera and Directional Light from the new scene (right-click >
   Delete in the Hierarchy window). We'll add our own lights later.
4. **File > Save As...** → save to `Assets/Enara/Scenes/Boot.unity`.

### 4.2 Create the GameRoot GameObject

1. In the Hierarchy window (left), right-click → **Create Empty**.
2. Rename it to `GameRoot`.
3. With `GameRoot` selected, in the Inspector click **Add Component**. Type these in the
   search box one at a time and add them:
   - `GameManager` (under Enara.Core)
   - `SaveSystem` (under Enara.Save)
   - `AudioManager` (under Enara.Audio)
   - `Fader` (under Enara.UI)
   - `SceneLoader` (under Enara.SceneFlow)
   - `ChapterDirector` (under Enara.SceneFlow)
   - `InputReader` (under Enara.Input)
   - `DebugCheats` (under Enara.Utility) — optional but useful

4. The `InputReader`'s `actionsAsset` slot is empty. Drag the
   `Assets/Enara/Resources/PlayerControls.inputactions` asset into it.

### 4.3 Add the AudioManager's AudioSources

The AudioManager needs three `AudioSource` components (for music, SFX, VO). The simplest way
is to put them as children of GameRoot.

1. Right-click `GameRoot` in Hierarchy → **Create Empty**. Rename it to `MusicSource`.
2. With `MusicSource` selected, **Add Component** → search `Audio Source` → add it.
3. Repeat twice more: create `SFXSource` and `VOSource`, each with an Audio Source.
4. Now select `GameRoot` again. In the AudioManager component, you'll see three empty slots:
   `musicSource`, `sfxSource`, `voSource`. Drag the three child GameObjects into them.
5. Drag the `AudioMixer` asset from the Project window into the AudioManager's `mixer` slot.

### 4.4 Add the Fader

The Fader needs a full-screen Image to fade the screen.

1. Right-click in Hierarchy → **UI > Canvas**. A Canvas appears (with an EventSystem).
2. Right-click the Canvas → **UI > Image**. Rename it to `Fader`.
3. Select `Fader`. In the Inspector:
   - **Rect Transform**: set anchors to stretch both (click the anchor box, hold Alt/Opt,
     click the bottom-right preset).
   - **Rect Transform** width and height: 0, 0 (with stretch anchors, 0 means full screen).
   - **Image** component: set color to black, alpha to 0 (transparent).
4. The Fader MonoBehaviour is already on `GameRoot`. Drag the `Fader` GameObject from the
   Hierarchy into GameRoot's `Fader` component... wait, no.

Actually let me re-read. `Fader` is a MonoBehaviour with `[RequireComponent(typeof(Image))]`.
So it needs to be on the same GameObject as an Image. Let me fix the setup:

- Select the `Fader` GameObject you just made (the one with the Image).
- **Add Component** → search `Fader` → add it. The Image is already there.
- Now this `Fader` GameObject has both Image + Fader. 

But earlier I told you to add Fader to GameRoot. Let me correct that — remove Fader from
GameRoot, and instead it lives on the `Fader` GameObject under the Canvas. The Fader script
will call `DontDestroyOnLoad` on its root (which is the Canvas... no wait, the Canvas
itself). Actually the Fader's `transform.root` is the Canvas. So `DontDestroyOnLoad` makes
the Canvas persist. That's fine, you probably want the Canvas to persist anyway.

**So the corrected steps:**
- Remove the `Fader` component from GameRoot (right-click it in Inspector → Remove Component).
- Select the `Fader` GameObject (child of Canvas) and add the `Fader` component there.

### 4.5 Add the ChapterDirector's chapter list

We'll create the actual chapter asset in Step 5 and wire it then. For now leave the
`ChapterDirector`'s `chapters` list empty.

### 4.6 Save and add Boot to Build Settings

1. **File > Save** (Cmd+S).
2. Top menu: **File > Build Profiles** (Unity 6 renamed Build Settings to Build Profiles).
3. Click **Add Open Scenes**. You'll see `Assets/Enara/Scenes/Boot.unity` appear at index 0.
4. Close the Build Profiles window.

**Verify**: Save the scene. Press Play (Cmd+P). The console should show no errors. The game
state should be `Boot`. Nothing happens yet because there are no chapters wired.

Press Play again to stop.

---

## Step 5: Create your first chapter

### 5.1 Create the ChapterDefinition asset

1. Right-click `Assets/Enara/Data/` (or make a `Chapters` subfolder) → **Create > Enara > Chapter**.
2. Name it `Chapter_Forest_Limp`.
3. Click it. In the Inspector:
   - **Id**: type `Forest_Limp` (this is the string the code uses).
   - **Display Name**: `The Limp` (whatever you want shown in HUD).
   - **Scene Path**: `Assets/Enara/Scenes/Forest_Limp.unity` (we'll create this scene next).
   - **Designer Notes**: optional — "Player wakes up injured, limps, QTE Jesus Prayer fires."

### 5.2 Wire it into the ChapterDirector

1. Open the Boot scene again (double-click it in the Project window).
2. Select `GameRoot`. In the ChapterDirector component, you'll see an empty `Chapters` list.
3. Click the **+** button. A new row appears.
4. Drag the `Chapter_Forest_Limp` asset from the Project window into the new row's slot.
5. **Check the `Start On Awake` checkbox** — this makes the game auto-start when you press Play
   from the Boot scene.

**Verify**: Boot scene's ChapterDirector now shows one chapter in the list and `Start On Awake`
is checked.

---

## Step 6: Create the Forest_Limp scene

This is your first actual gameplay scene. The player will walk (limping) and the QTE system
will fire periodically.

### 6.1 Create the scene

1. **File > New Scene** → **Basic (URP)** → Create.
2. Delete the default Main Camera (we'll add our own on the player).
3. Keep the Directional Light — it's the sun.
4. **File > Save As...** → `Assets/Enara/Scenes/Forest_Limp.unity`.

### 6.2 Add a floor

1. In Hierarchy, right-click → **3D Object > Plane**.
2. Rename it to `Ground`.
3. In Inspector, set Transform to position (0, 0, 0), rotation (0, 0, 0), scale (3, 1, 3).
   (A bigger floor so the player can't accidentally walk off.)
4. The Plane already has a Mesh Collider, so the player won't fall through.

### 6.3 Add the player GameObject

This is the most fiddly step. Take your time.

1. Right-click in Hierarchy → **Create Empty**. Rename to `Player`.
2. Set Player's Transform: position (0, 1, 0), rotation (0, 0, 0), scale (1, 1, 1).
3. With Player selected, **Add Component** → search `Character Controller` → add it.
   - In the Character Controller component, set:
     - **Height**: 2
     - **Radius**: 0.4
     - **Center**: (0, 1, 0) — so the controller's capsule sits on the ground properly.
4. **Add Component** → search `Player Controller` (under Enara.Player) → add it.
5. **Add Component** → search `First Person Look` → add it.
6. **Add Component** → search `Footstep Controller` → add it (optional, but you have it).
7. Right-click `Player` in Hierarchy → **Camera**. This makes a Camera as a child of Player.
8. Set the Camera's Transform: position (0, 0.6, 0), rotation (0, 0, 0). (This puts the camera
   at eye height ~1.6m above the player's feet, since the player is at y=1.)
9. Select `Player` again. In the `First Person Look` component:
   - Drag the Camera (the child) into the `Camera Transform` slot.
   - Drag `Player` itself into the `Body Transform` slot (drag from Hierarchy).
10. In the `Player Controller` component, leave everything default for now. (Look sensitivity
    and walk speed come from `GameSettings`.)
11. In the `Footstep Controller` component: drag `Player` into `player`, and drag `GameRoot`'s
    AudioManager child... wait, AudioManager is in Boot scene, not this one. Skip footsteps
    for now — they need audio clips anyway. You can disable the Footstep Controller component
    (uncheck the checkbox next to its name in Inspector).

### 6.4 Add a Tag for the player

The TriggerZone, WitnessTrigger, etc. all filter by tag "Player". The player GameObject needs
that tag.

1. Select `Player`.
2. At the very top of the Inspector, there's a **Tag** dropdown (says "Untagged").
3. Click it → **Player** (built-in Unity tag).

### 6.5 Add the QTE system

1. Right-click in Hierarchy → **Create Empty**. Rename to `QTESystem`.
2. **Add Component** → search `Quick Time Event System` → add it.
3. In the component:
   - Drag the `Player` GameObject into the `Player` slot.
   - Leave `Ui` and `Subtitles` empty for now (we'll create them next).
4. **Add Component** → search `Morality Tracker` → add it to `QTESystem` (or anywhere —
   it doesn't need references for now).

### 6.6 Add the UI Canvas for subtitles + QTE prompt

1. Right-click in Hierarchy → **UI > Canvas**. (A Canvas + EventSystem appears.)
2. Click the Canvas. In the **Canvas Scaler** component:
   - **UI Scale Mode**: change to **Scale With Screen Size**.
   - **Reference Resolution**: X=1920, Y=1080.
3. Right-click the Canvas → **UI > Panel**. Rename to `SubtitlesPanel`.
4. With `SubtitlesPanel` selected:
   - **Rect Transform**: set anchors to bottom-center (click the anchor box, hold Alt/Opt,
     click the bottom-center preset).
   - Set Rect Transform height to 100, position Y to 100.
   - **Image** component: set color to black with alpha 80 (semi-transparent).
5. Right-click `SubtitlesPanel` → **UI > Text - TextMeshPro**. Rename to `SubtitlesLabel`.
   - If Unity asks you to "Import TMP Essentials", click **Import TMP Essentials** then close.
   - With `SubtitlesLabel` selected, set its Rect Transform to stretch both (Alt+click
     bottom-right preset).
   - In the TextMeshPro component, set:
     - **Alignment**: center, middle.
     - **Font Size**: 36.
     - **Color**: white.
6. Select `SubtitlesPanel` again. **Add Component** → search `Subtitles` (under Enara.UI) →
   add it. Drag `SubtitlesLabel` into its `Label` slot.

### 6.7 Add the QTE UI

1. Right-click the Canvas → **UI > Panel**. Rename to `QtePanel`.
2. Set its anchors to middle-center, set width to 400, height to 200, position Y to 200.
3. Set the Image color to dark gray with alpha 200.
4. Right-click `QtePanel` → **UI > Text - TextMeshPro**. Rename to `PromptLabel`.
   - Anchors: top stretch. Position Y = -20.
   - Alignment: center, middle. Font size: 48. Color: white.
5. Right-click `QtePanel` → **UI > Text - TextMeshPro**. Rename to `KeyHintLabel`.
   - Anchors: bottom stretch. Position Y = 20.
   - Alignment: center, middle. Font size: 24. Color: yellow.
6. Right-click `QtePanel` → **UI > Image**. Rename to `TimerFill`.
   - Anchors: bottom stretch. Width: 0 (fills the panel width). Height: 10.
   - Image Type: **Filled**. Fill Method: **Horizontal**. Fill Origin: **Left**.
   - Fill Amount: 1. Color: green.
7. Select `QtePanel`. **Add Component** → search `Qte UI` (under Enara.QTE) → add it.
   - Drag `QtePanel` itself into the `Root` slot.
   - Drag `PromptLabel` into `Prompt Label`.
   - Drag `KeyHintLabel` into `Key Hint Label`.
   - Drag `TimerFill` into `Timer Fill`.
8. Now select `QTESystem` (the GameObject from step 6.5). In its `Quick Time Event System`
   component, drag `QtePanel` into the `Ui` slot, and `SubtitlesPanel` into the `Subtitles`
   slot.

### 6.8 Save the scene

**File > Save** (Cmd+S).

---

## Step 7: Test it!

1. Open the Boot scene (double-click `Assets/Enara/Scenes/Boot.unity` in the Project window).
   - You must always play from Boot, not from Forest_Limp, because the persistent singletons
     (InputReader, GameManager, AudioManager, etc.) live in Boot.
2. Press **Play** (Cmd+P) at the top center of the editor.
3. **What should happen:**
   - The screen briefly fades (Fader).
   - After ~0.5 seconds, the Forest_Limp scene loads.
   - You can move with WASD / arrows. The mouse rotates the camera.
   - The player walks slowly (limping).
   - Every 4-8 seconds, a QTE panel appears at the top of the screen showing a word of the
     Jesus Prayer and the key to press (Space / E / F / Q).
   - If you press the right key in time, the word appears at the bottom in the Subtitles panel.
   - After 8 successes, the player's limp is removed (you walk faster) and QTEs stop.
4. Press Play again to stop.

### 7.1 If something doesn't work

- **Player can't move**: Check the console. Is `InputReader` missing? Select GameRoot in Boot
  scene — does it have an InputReader component with the actionsAsset wired?
- **Mouse doesn't look**: Select Player. Does FirstPersonLook have both `Camera Transform` and
  `Body Transform` filled?
- **QTE never fires**: Select QTESystem. Are `Ui` and `Subtitles` slots filled? Check the
  console for errors.
- **Player falls through floor**: Select Ground. Does it have a Mesh Collider (the Plane
  creates one automatically)? Is the Player's CharacterController center set to (0, 1, 0)?
- **Black screen after fade**: The Fader's Image might be opaque. Set its alpha to 0.
- **"No GameSettings.asset in Resources"**: You didn't move the GameSettings asset into
  `Assets/Enara/Resources/`. Do that.

If you see a red error in the console, copy-paste the message and ask opencode what it means.

---

## Step 8: Tweak the GameSettings

1. Select the `GameSettings` asset in `Assets/Enara/Resources/`.
2. In the Inspector, try:
   - Set `Qte Window Seconds` to 2.0 (gives you more time to press the key).
   - Set `Qtes Until Healed` to 3 (so you finish the loop faster while testing).
   - Set `Limp Speed` to 1.5 (less painfully slow while testing).
3. Press Play. The new values apply immediately (no need to restart Unity).

---

## Step 9: What to do next

You now have the core systems running. From here, pick what interests you:

### Build Path 1 (Old Man) — most detailed in the README
- Follow `docs/SCENE_GUIDE.md` "Path 1" section.
- Add an angel with `HolyLightController`.
- Add the idol statue with `LookAtPlayer` + `IInteractable`.
- Author the idol dialogue graph (right-click `Assets/Enara/Data/Dialogue/` → **Create > Enara > Dialogue Graph**).
- Add the Old Man with `CompanionController` (needs NavMesh bake — see below).
- Wire the `WakeUpSequence` at the end.

### Bake a NavMesh (needed for NPCs)

1. Make sure your ground has a collider.
2. Select any static obstacle (walls, trees, etc.) and in the Inspector top-right, set
   **Navigation Static** = true, **Navigation Area** = **Not Walkable**.
3. Top menu: **Window > AI > Navigation**. Click the **Bake** tab.
4. Click **Bake** at the bottom right. A blue NavMesh appears on the floor.
5. Now `WaypointPatrol` and `CompanionController` can pathfind.

### Add a TriggerZone

1. Create an empty GameObject, add a Box Collider sized to cover the trigger area.
2. **Add Component** → search `Trigger Zone` → add it.
3. In the Inspector, **Is Trigger** on the Box Collider must be checked (it usually is
   automatically when you add TriggerZone).
4. In the `On Enter` UnityEvent list, click **+**, then drag any GameObject with a method you
   want to call (e.g. a `HolyLightController`'s GameObject). Pick the method from the dropdown
   (e.g. `HolyLightController.Appear`).

### Author a Dialogue Graph

1. Right-click `Assets/Enara/Data/Dialogue/` → **Create > Enara > Dialogue Graph**. Name it
   `IdolDialogue`.
2. Select it. In the Inspector, click **+** on the **Nodes** list. Add a few nodes:
   - **Entry Node Id**: `idol_intro`.
   - **Speaker Name**: `Idol`.
   - **Text**: "I can save you, child. Only bow to me."
3. Right-click `Assets/Enara/Data/Dialogue/` → **Create > Enara > Dialogue Node**. Create one
   per beat. Wire `Next Node Id` to chain them, or add **Choices**.
4. Drop the `DialogueGraph` asset onto an `IInteractable` component somewhere in the scene.

### Build a cutscene (later)

Cutscenes use Unity Timeline. Watch the "Getting started with Timeline" tutorial in
`docs/LEARNING.md` Hour 8 before attempting this.

---

## Quick reference: every script and what it does

| Folder | Script | What it does | When you'd add it to a scene |
|---|---|---|---|
| Core | `GameManager` | Singleton; owns GameStateMachine; persists. | Boot scene, once. |
| Core | `GameStateMachine` | Tracks Boot/Cutscene/Exploration/Qte/Dialogue/Menu/Paused/Ending. | (Owned by GameManager.) |
| Core | `EventBus` | Typed pub/sub for cross-system signals. | (Static — use anywhere.) |
| Core | `ServiceLocator` | Lookup for non-MonoBehaviour services. | (Static.) |
| Core | `GameSettings` | SO with tunables. | Create the asset once. |
| Core | `ChapterDefinition` | SO describing one chapter. | Create one per chapter. |
| SceneFlow | `ChapterDirector` | Ordered list of chapters; Advance/GoToChapter. | Boot scene. |
| SceneFlow | `SceneLoader` | Async scene load with fade. | Boot scene. |
| Player | `PlayerController` | First-person movement; Limping flag. | Every gameplay scene's Player. |
| Player | `FirstPersonLook` | Mouse look (yaw on body, pitch on camera). | On Player. |
| Interaction | `IInteractable` | Interface for things the player can interact with. | On the idol, icon of Christ, NPCs, doors. |
| Interaction | `Interactor` | Camera raycast to find interactables. | On Player (or its camera). |
| Interaction | `InteractionPromptUI` | "Press E to talk" prompt. | On a UI Canvas. |
| QTE | `QuickTimeEventSystem` | The Jesus Prayer loop; heals player after N successes. | Forest_Limp scene. |
| QTE | `QteUI` | Prompt + timer bar. | On a UI Canvas. |
| Dialogue | `DialogueGraph` | SO conversation data. | Author as asset. |
| Dialogue | `DialogueRunner` | Plays a graph node-by-node. | One per scene with dialogue. |
| Choice | `ChoicePresenter` | Spawns buttons for choices. | On a UI Canvas. |
| Audio | `AudioManager` | Singleton music/SFX/VO via AudioMixer. | Boot scene, once. |
| UI | `Subtitles` | TMP text for one-liner subtitles. | On a UI Canvas in each scene. |
| UI | `Fader` | Full-screen black fade singleton. | On a UI Canvas in Boot. |
| UI | `HUD` | Optional chapter/objective display. | On a UI Canvas. |
| Save | `SaveSystem` | JSON save with flags + stats. | Boot scene, once. |
| Input | `InputReader` | Singleton wrapping PlayerControls.inputactions. | Boot scene, once. |
| Cutscene | `CutscenePlayer` | PlayableDirector wrapper with fade + state lock. | On any cutscene GameObject. |
| Cutscene | `CutsceneSignalReceiver` | Forwards Timeline signals as UnityEvents. | Same GameObject as PlayableDirector. |
| Cutscene | `CameraShake` | Procedural shake. | On the camera. |
| Cutscene | `SubtitleSequencePlayer` | Plays a list of subtitle lines in order. | On a UI Canvas. |
| NPC | `NPCController` | Base class. | On any NPC. |
| NPC | `WaypointPatrol` | NavMeshAgent loop patrol. | On Babel slaves. |
| NPC | `LookAtPlayer` | Smoothly rotate to face player. | On the idol's head bone, ambient NPCs. |
| NPC | `CompanionController` | NavMeshAgent follower for the Old Man. | On the Old Man. |
| World | `TriggerZone` | Tag-filtered collider with onEnter/onExit UnityEvents. | Everywhere you need "step here to X". |
| World | `PathBranchRouter` | Routes player to Path1/2/3 chapters. | Forest_Paths scene. |
| World | `WitnessTrigger` | Fires when player looks at it for N seconds. | On the icon of Christ, the idol. |
| World | `MoodLightingController` | Gradually darkens ambient. | Path 1 scene. |
| World | `FootstepController` | Plays footstep SFX (limp vs walk). | On Player. |
| World | `GlideController` | Hold E to glide down (Path 2 tower). | On Player in Path 2. |
| World | `TowerProgressionController` | Stage-by-stage tower reveal. | Path 2 scene. |
| World | `LocalizationProvider` | SO for multi-language strings (Babel VO). | Create as asset. |
| Story | `MoralityTracker` | nearnessToGod + temptedAway floats. | Boot scene, once. |
| Story | `EndingDirector` | Picks BadEasyPath / Denial / Resolution. | Ending scene. |
| Story | `PlayerAppearance` | Applies sigil tattoo marker. | On Player. |
| Story | `ScriptureReciter` | SO with verses for Psalm 50 etc. | Author as asset. |
| Story | `HolyLightController` | Appear/disappear for angel + Mary. | On the angel / Mary GameObject. |
| Story | `WakeUpSequence` | Orchestrates the noose wake-up. | Path 1 scene. |
| Story | `ChapterCheckpoint` | Saves the current chapter on enter. | Place throughout scenes. |
| Story | `MiracleEvent` | SO for one-shot miracle with effects + SFX. | Author as asset. |
| Menu | `MainMenu` | Title screen. | MainMenu scene. |
| Menu | `PauseMenu` | Escape pause overlay. | Persistent Canvas in Boot. |
| Menu | `SettingsMenu` | Volume + sensitivity sliders. | MainMenu scene. |
| Menu | `LoadingScreen` | Progress bar. | Persistent Canvas. |
| Menu | `CreditsRoll` | Scrolling credits. | Ending scene Canvas. |
| Menu | `DecisionSummaryUI` | End-game choice summary. | Ending scene Canvas. |
| Utility | `DebugCheats` | F1-F12 dev keys. | Boot scene. |
| Editor | `BuildTools` | CLI build entry point. | (Editor-only.) |
| Editor | `ChapterValidator` | Menu item to validate chapters. | (Editor-only.) |
| Editor | `ReadOnlyAttribute` | Read-only inspector fields. | (Editor-only.) |

---

## Questions you might still have

**Q: I press Play and nothing happens / the screen stays black.**
A: Make sure you're playing the **Boot scene**, not Forest_Limp. Make sure ChapterDirector's
`Start On Awake` is checked. Make sure the Fader's Image alpha is 0.

**Q: The console shows a red error about a missing script.**
A: Copy the full error message and paste it to opencode. Most are easy fixes.

**Q: How do I add a new ScriptableObject asset?**
A: Right-click in the Project window → **Create > Enara > [the type]**. The menu items come
from the `[CreateAssetMenu]` attribute on each SO class.

**Q: How do I commit my work?**
A: In a terminal:
```sh
cd /Users/eliaa/Code/Enara
git add -A
git commit -m "Add the idol dialogue graph"
git push
```
Never commit the `Library/`, `Temp/`, `Obj/`, `Build/`, or `Logs/` folders (they're in
`.gitignore` already, so `git add -A` is safe).

**Q: Where do I get art / audio?**
A: See `docs/ASSETS.md` — Kenney.nl, Quaternius, Freesound.org, Incompetech are all free.

**Q: I want to make a change to a script but I'm scared of breaking things.**
A: Make a copy of the file first. Or use git: `git stash` saves your uncommitted changes and
you can restore them with `git stash pop`. Worst case, `git checkout -- <file>` reverts.

---

## You finished the tutorial. What now?

Read `docs/ROADMAP.md` to see what's next. The recommended order:

1. Build out the rest of `Forest_Limp` (a few trees, a path, some atmosphere).
2. Build `Forest_Paths` with three trigger zones routing to three chapters.
3. Build `Path 1 (Old Man)` — it's the most detailed in the README and exercises the most
   systems (dialogue, choice, NPC companion, mood lighting, wake-up sequence).
4. Then Path 3 (Monastery) — simpler, mostly recitation.
5. Then Path 2 (Babel) — needs more art (town, tower) and multi-language VO.
6. Then the Ending.

Take it one scene at a time. Commit after every working scene. Ask opencode when you get stuck.
