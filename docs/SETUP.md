# Setup

First-time setup for the Enara Unity project.

## 1. Install Unity

1. Install **Unity Hub** from <https://unity.com/download>.
2. Open Unity Hub, go to **Installs > Install Editor**.
3. Pick **Unity 6.0.5 (`6000.5.2f1`)** - this is the exact version this project uses.
   (If it's not in the list, click "Archive" on the Unity download page to find older builds.)
4. When prompted for modules, check at least:
   - **Documentation**
   - **Build Support** for your platform (Windows / Mac / Linux)
   - Optional: **Android Build Support** if you ever want a mobile build

## 2. Open the project

1. In Unity Hub: **Open > Add project from disk**.
2. Pick the folder containing this README (`/Users/eliaa/Code/Enara` or wherever you cloned).
3. The first open reimports all assets and recompiles scripts. **This takes 2-5 minutes.**
4. When it's done, the console (`Window > General > Console` or Ctrl+Shift+C) should have no
   red errors. Yellow warnings are fine.

If you see red compile errors, the most common causes are:

- Wrong Unity version. Check `ProjectSettings/ProjectVersion.txt` - it must say `6000.5.2f1`.
- A package is missing. Open `Window > Package Manager` and verify the inputsystem /
  render-pipelines.universal packages are installed (they should be - they're in
  `Packages/manifest.json`).

## 3. Create the GameSettings asset

This is the global tunables file. The code already references it but the asset itself
must be created once:

1. In the Project window, right-click the folder `Assets/Enara/Settings/`.
2. **Create > Enara > GameSettings**.
3. Name it `GameSettings`.
4. Click it and tweak the values (QTE timings, walk/limp speeds, volumes).

## 4. Create the AudioMixer

1. Right-click `Assets/Enara/Settings/`.
2. **Create > Audio Mixer**. Name it `AudioMixer`.
3. Open it (`Window > Audio > Audio Mixer` if it doesn't open automatically).
4. In the mixer window, click **+** next to **Groups** and add three children of Master:
   `Music`, `SFX`, `VO`. (Master already exists.)
5. For each group, right-click the volume field in the inspector and **Expose '...Volume'**.
   Rename the exposed parameters to `MasterVolume`, `MusicVolume`, `SfxVolume` (the AudioManager
   looks up these exact names).

## 5. Open the Boot scene

There isn't a Boot scene yet - you'll create it in step 6 of `docs/SCENE_GUIDE.md`. Once it
exists, open `Assets/Enara/Scenes/Boot.unity` and press **Play** (Cmd+P / Ctrl+P).

## 6. CLI / headless build (optional)

For automated builds (CI, build servers):

```sh
UNITY="/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity"
"$UNITY" -batchmode -nographics -projectPath . -executeMethod Enara.Editor.BuildTools.BuildStandalone -quit -logFile build.log
```

(The `BuildTools` editor class isn't written yet - add it when you need it. See ROADMAP.)

## 7. Editor preferences (optional but recommended)

- **External script editor**: `Edit > Preferences > External Tools`. Pick VS Code, Rider,
  or Visual Studio. Make sure "Generate .csproj files" is enabled for: Player, Editor.
- **Asset serialization**: already set to `ForceText` (good for git diffs - check
  `ProjectSettings/ProjectSettings.asset`'s `m_ExternalVersionControlSupport`).

## 8. Verify the input system

1. Open `Assets/Enara/Resources/PlayerControls.inputactions`.
2. The editor should show two action maps: **Player** and **UI**.
3. Click **Save** if there are unsaved changes (it shouldn't ask, but just in case).
4. If the bindings look wrong, you can edit them in this window - the format is the standard
   Unity Input System editor.

## Troubleshooting

**"All compiler errors have to be fixed before you can enter playmode!"**
Open the console and fix the topmost red error first. Often a single typo cascades.

**Console says "No GameSettings.asset in Resources"**
You skipped step 3 above. Create the asset.

**Mouse doesn't move the camera**
Make sure the `InputReader` component is in the scene and that the `PlayerControls` asset is
dragged into its `actionsAsset` slot (or just leave it null - it auto-loads from Resources).

**Player falls through the floor**
The floor needs a Collider (Box / Mesh). The player needs a CharacterController (the
PlayerController component requires one and adds it automatically).

**More help**: see `docs/LEARNING.md` for tutorials.
