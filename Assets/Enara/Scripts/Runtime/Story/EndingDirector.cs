using System;
using UnityEngine;
using Enara.Core;

namespace Enara.Story
{
    /// <summary>
    /// Picks which ending plays based on save flags + <see cref="MoralityTracker"/> state.
    /// Call <see cref="ResolveEnding"/> when the player reaches the ending chapter.
    ///
    /// Endings (matching the README):
    /// - <see cref="EndingType.BadEasyPath"/>: player bowed to the idol, took the easy path
    ///   (cutscene: divorce / death / sigil tattoo).
    /// - <see cref="EndingType.Denial"/>: player completed a path but denies everything in the
    ///   modern village.
    /// - <see cref="EndingType.Resolution"/>: a softer resolution based on high nearness.
    /// </summary>
    public sealed class EndingDirector : MonoBehaviour
    {
        public enum EndingType { BadEasyPath, Denial, Resolution }

        [SerializeField] private MoralityTracker morality;
        [SerializeField] private Cutscene.CutscenePlayer badEasyPathCutscene;
        [SerializeField] private Cutscene.CutscenePlayer denialCutscene;
        [SerializeField] private Cutscene.CutscenePlayer resolutionCutscene;
        [SerializeField] private float nearnessThresholdForResolution = 70f;

        /// <summary>Resolved ending (null until <see cref="ResolveEnding"/> is called).</summary>
        public EndingType? ResolvedEnding { get; private set; }

        /// <summary>Compute and play the appropriate ending. Returns the chosen ending.</summary>
        public EndingType ResolveEnding()
        {
            EndingType choice;
            var save = Save.SaveSystem.Instance;
            if (save != null && save.HasFlag("took_easy_path"))
                choice = EndingType.BadEasyPath;
            else if (morality != null && morality.NearnessToGod >= nearnessThresholdForResolution)
                choice = EndingType.Resolution;
            else
                choice = EndingType.Denial;

            ResolvedEnding = choice;
            GameManager.Instance?.SetState(GameState.Ending);

            switch (choice)
            {
                case EndingType.BadEasyPath: badEasyPathCutscene?.Play(); break;
                case EndingType.Denial:      denialCutscene?.Play(); break;
                case EndingType.Resolution:   resolutionCutscene?.Play(); break;
            }
            return choice;
        }
    }
}
