# Learning Resources

You said you don't know much about Unity. Here's a curated path to learn what you need for
this project, in order. Everything here is free.

## Hour 1: The absolute basics

Watch the official "Get started with Unity" tutorial series. It's about 60 minutes total:

- **Unity Learn - "Use the Unity Editor" (free, sign in with Unity ID)**
  - URL: <https://learn.unity.com/course/use-the-unity-editor>
  - Covers: project setup, the editor windows, scene view, game view, basic GameObjects.

After this, you should be able to:
- Open a project, navigate the Scene view (pan / orbit / zoom).
- Add a Cube, move it, rotate it, scale it.
- Press Play.

## Hour 2-3: C# in Unity

- **"Unity C# Scripting - Beginner to Pro" on YouTube** (search the title)
  - Or: the official <https://learn.unity.com/tutorial/writing-your-first-script>
- Key concepts you must understand before reading the Enara code:
  - `MonoBehaviour`, `Awake`, `Start`, `Update`
  - `[SerializeField] private` vs `public` fields
  - `GetComponent<T>()`, `FindObjectOfType<T>()`
  - `Debug.Log`
  - Coroutines (`IEnumerator`, `yield return`)

## Hour 4-5: The New Input System

This project uses the new Input System (not the legacy one). Watch:

- **Unity's official "Input System" video**: search YouTube for "Unity Input System 1.x tutorial"
- Read the manual: <https://docs.unity3d.com/Packages/com.unity.inputsystem@1.9/manual/Actions.html>

You should be able to:
- Open the `.inputactions` editor and add a binding.
- Explain what a "Control Scheme", "Action Map", and "Action" are.
- Know the difference between `Value` and `Button` action types.

## Hour 6: ScriptableObjects

The Enara code uses ScriptableObjects for `ChapterDefinition`, `DialogueGraph`, `DialogueNode`,
and `GameSettings`. Watch:

- **"Game Architecture with Scriptable Objects" - Ryan Hipple (Unite 2017)** on YouTube.
  Old but still the canonical talk.

You should be able to:
- Explain the difference between a MonoBehaviour and a ScriptableObject.
- Create a SO asset via `CreateAssetMenu`.
- Drag a SO asset into a MonoBehaviour's inspector field.

## Hour 7: URP (Universal Render Pipeline)

- **Unity Learn "Introduction to URP"** - <https://learn.unity.com/tutorial/introduction-to-universal-render-pipeline>
- Just enough to know: URP vs Built-in, the URP Asset, lights, post-processing.

## Hour 8: Timeline & Cinemachine

For the driving intro and cutscenes you'll want Timeline:

- **Unity Learn "Getting started with Timeline"** - <https://learn.unity.com/course/getting-started-with-timeline>
- **Cinemachine** - install it via Package Manager (`com.unity.cinemachine`) then watch
  Unity's "Cinemachine" tutorial on YouTube.

## Hour 9: TextMeshPro

- **Unity's official "TextMeshPro" tutorial** on YouTube.
- All UI text in Enara uses TMP. The codebase uses `TMP_Text` (the base class) so both
  `TextMeshProUGUI` and `TextMeshPro` work.

## Hour 10+: Practice

Open the Enara project and:

1. Run the Boot scene. See no errors.
2. Create the `GameSettings.asset` (SETUP.md step 3).
3. Build the `Forest_Limp.unity` scene (SCENE_GUIDE.md).
4. Get a QTE to fire. Press the right key. See the Jesus Prayer word appear in subtitles.

That's your first milestone. Once you've done that, the rest of the project is mostly
content authoring (dialogue, art, audio) - the systems are already there.

## Reference docs to bookmark

- Unity Scripting API: <https://docs.unity3d.com/ScriptReference/>
- Unity 6 manual: <https://docs.unity3d.com/6000.0/Documentation/Manual/index.html>
- Input System manual: <https://docs.unity3d.com/Packages/com.unity.inputsystem@1.9/manual/index.html>
- URP manual: <https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/index.html>
- TextMeshPro docs: <https://docs.unity3d.com/Packages/com.unity.ugui@2.5/manual/TextMeshPro/index.html>

## Where to ask for help

- **Unity Forum** (slower but deep): <https://forum.unity.com/>
- **Unity Discord**: <https://discord.gg/unity> - very active.
- **Stack Overflow** with `[unity3d]` tag.
- **Reddit** r/Unity3D and r/Unity - browse for similar problems first.

## Tips

- **Read your console.** The compiler is your friend. When something breaks, read the error,
  double-click it, and fix the topmost one first.
- **Save scene + project often.** Ctrl+S, Ctrl+Shift+S (save all). Unity does not autosave.
- **Don't edit .unity / .prefab / .asset YAML by hand** unless you really know what you're
  doing. Use the Editor.
- **Commit small.** Every time something works, commit. You can always amend later.
