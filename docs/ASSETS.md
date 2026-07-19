# Asset List

What art / audio / data assets the game needs, and where they go. Use this as a shopping list.

## Where things live

| Type | Folder | Notes |
|---|---|---|
| 3D models | `Assets/Enara/Art/Models/` | FBX, OBJ. Commit only if < ~256 KB; otherwise use git LFS. |
| Textures | `Assets/Enara/Art/Textures/` | PNG. Same size rule. |
| Materials | `Assets/Enara/Art/Materials/` | URP materials only (Lit / Unlit). |
| Music | `Assets/Enara/Audio/Music/` | WAV for short loops, OGG for long tracks. |
| SFX | `Assets/Enara/Audio/SFX/` | WAV. |
| VO | `Assets/Enara/Audio/VO/` | WAV. Filename pattern: `vo_chapter_speaker_line.wav`. |
| Animations | `Assets/Enara/Animations/` | `.anim`, `.controller`. |
| Fonts | `Assets/Enara/Fonts/` | TTF / OTF. TMP imports them. |
| Dialogue data | `Assets/Enara/Data/Dialogue/` | `DialogueGraph` SO instances. |
| Chapter data | `Assets/Enara/Data/Chapters/` | `ChapterDefinition` SO instances. |
| Prefabs | `Assets/Enara/Prefabs/` | Reusable GameObjects. |

## Per-scene asset checklist

### Intro_Drive
- [ ] Car model
- [ ] Road / forest environment art
- [ ] Metal music track (intro)
- [ ] Crash SFX

### Forest_Limp
- [ ] Forest environment (trees, ground, fog)
- [ ] Limping player animation (or just rely on slow movement)
- [ ] Jesus Prayer VO (optional - subtitles are fine for v1)
- [ ] Per-word QTE success SFX

### Path 1 (Old Man)
- [ ] Gold idol statue (turning head anim optional)
- [ ] Old man character
- [ ] Virgin Mary character / light beam
- [ ] Noose prop
- [ ] Forest continuation art

### Path 2 (Babel)
- [ ] Town on a mountain (modular buildings)
- [ ] NPCs (slaves hauling dirt)
- [ ] Tower under construction
- [ ] Multi-language VO (different languages for the punishment scene)

### Path 3 (Monastery)
- [ ] Monastery exterior + interior
- [ ] Icon of Christ (interactable)
- [ ] Angelic chant audio loop
- [ ] Empty pews / dust motes for atmosphere

### Ending
- [ ] Modern village / hospital scene
- [ ] Final denial VO

## Free asset sources (legitimate)

When you don't have custom art yet:

- **Kenney.nl** - free CC0 game art (3D + 2D + audio): <https://kenney.nl>
- **Unity Asset Store** - free section has lots of low-poly packs
- **Quaternius** - free low-poly nature / character packs: <https://quaternius.com>
- **Poly Pizza** - free low-poly models: <https://poly.pizza>
- **Freesound.org** - free SFX (check license per clip)
- **Incompetech** - Kevin MacLeod's free music (CC-BY attribution required)

## Avoid

- Anything from a random Google image search - copyright.
- "Ripped" game assets (e.g. extracted from a commercial game).
- Large binary files committed to git without LFS (the .gitignore tries to filter, but check).
