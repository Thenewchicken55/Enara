# Enara

Enara is the name of the video game. In Arabic, it means Light or Enlightenment.

The theme of the game is falling and getting back up, when it comes to our Spiritual Journey to God. The user starts off far from God. Then a tribulation happens, then they go back to God, and feel slightly better, then forsakes Him and falls back down in tribulation.

## Documentation

This is a Unity 6 project. If you're new to it, read in this order:

1. `docs/SETUP.md` - install Unity, open the project, first-time setup.
2. `docs/TUTORIAL_GETTING_STARTED.md` - **start here if you're new to Unity** - step-by-step from cloning the repo to a playable scene.
3. `docs/LEARNING.md` - curated Unity + C# learning path.
4. `docs/ARCHITECTURE.md` - how the code is organized, what each system does.
5. `docs/ROADMAP.md` - what's done, what's next, in order.
6. `docs/SCENE_GUIDE.md` - click-by-click instructions for wiring each scene in the Unity Editor.
7. `docs/ASSETS.md` - what art / audio / data assets you still need to create or source.
8. `AGENTS.md` - conventions for AI assistants (and humans) editing this repo.

## Plot

- The game starts off driving late at night (in forest/hills) with some metal music playing or something.
  - maybe here they see a sign or something pointing to camp?

- Player crashes (this is unpreventable cut scene)
- Player is now injured and now limps to try to find help.
  - However every few seconds they get a quick time event where the player has to click a button in due time. Sometimes it could be any key on the keyboard. If they get it right, they say the Jesus prayer: `Lord Jesus Christ, Son of God, have mercy on me, a sinner.`
  - After a while these quick time events stop happening because the player is healed. He doesn't "need" the Lord and forsakes Him.

- They go up and into the forest.
- They are presented with three paths, each has it's own tribulations.

#### Path 1: The Old Man

- The player is walking and encounters a light, an angel
- The angel warns the user not to talk to any one or **thing**
- Soon enough the player encounters an Idol. A statue. Made of Gold.
  - It can turn it's head and talk to the player
  - It's persuading the player to do something. To help the player...
  - Maybe player has choices of what to say to it
    - they can either go back to the jesus prayer, or take the "easy path"
    - you have to bow down to him to do this
    - The easy path is you get rescued but lose your happiness
    - the game basically ends with a cutscene Divorce. Death. Etc
    - but i think it would be cool if the peroson gets tatted with some sigil

- Assuming the player ignores the Idol, they can keep going. Then they encounter an Old man, who needs help
- They don't really have a choice but to walk with the old man and talk with him.
- They keep walking. Screen gets darker and darker.
- Then they see another Light. This time it's a woman.
  - "Leave him alone. Go, now"
  - The old man leaves.
  - _Turns to the player_ "Do you know where you are? Wake up"
  - "Look around you. Wake up"
  - "Lord have mercy, Wake Up!"

- Player finally wakes up and realizes they are in a noose. The Old man was a temptation. they were really close to ending themselves.
- The Woman -- The Virgin Mary -- disappears.
- Player gathers themselves and keeps going... IDK
  - TODO

#### Path 2: Tower of Babel

- Player ends up in a town. Everyone in this town is so distant from God.
- The Town is built upon a mountain.
- There is slavery and they keep hauling Dirt and stones up the mountain?
  - What's going on here?

- Player encounters poverty. Thieves. Sinners... etc.
- The player is just looking for help? the leader is at the top they say.
- Soon enough we realize they are trying to overcome God. They are building a Mountain (tower) to God.
- They actually got really far.
- Continue with tower of Babel story... Everyone gets punished idk. TODO
  - I need voice actors here, in different languages.

- Idk glide down from the tower trying to get home or something?

#### Path 3: TODO

- the player encounters a monastery.
- they can hear loud angelic chants of the liturgy
- ... but church is empty. As a matter of fact the only thing there is the icon of christ
- interacting with the icon, the player says psalm 50
- Even among the miracles they see even the pious get tribulations. Even harder tribulations

#### Ending

- I'm thinking the player somehow escapes the 3 Paths. Finds modern village, Hospital, or some help.
- Then they deny any of the things the player went through are actually real.
- Then resolutoin

## Current status

See `docs/ROADMAP.md` for the full milestone breakdown. In short:

- **Done**: folder structure, all core C# gameplay systems (game state, player controller,
  QTE / Jesus Prayer loop, dialogue, choices, audio, save, fade, subtitles, input).
- **Next (you)**: open the project in Unity, follow `docs/SCENE_GUIDE.md` to create the Boot
  scene, the GameSettings asset, the AudioMixer, and the first playable `Forest_Limp` scene.

Every C# script is documented with XML `///` comments - hover any class or method in your IDE
to see what it does.

