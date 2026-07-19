using UnityEngine;
using Enara.Core;

namespace Enara.Story
{
    /// <summary>
    /// Two-axis moral state of the player. The README's narrative arc moves the player closer
    /// to and farther from God. We track this so the ending can change tone based on accumulated
    /// choices - not a binary good/bad, but a softer "where is the player spiritually right now".
    ///
    /// - <see cref="nearnessToGod"/>: 0..100. Starts low (player is "far from God"). Goes up
    ///   when they recite the prayer, refuse the idol, accept Mary's wake-up call.
    /// - <see cref="temptedAway"/>: 0..100. Goes up when they bow to the idol, take the easy
    ///   path, ignore warnings.
    ///
    /// Use <see cref="AdjustNearness"/> / <see cref="AdjustTemptation"/> from event subscribers
    /// or from <see cref="ChoicePresenter"/>'s callbacks.
    /// </summary>
    public sealed class MoralityTracker : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)] private float startingNearness = 20f;
        [SerializeField, Range(0f, 100f)] private float startingTemptation = 10f;

        public float NearnessToGod { get; private set; }
        public float TemptedAway { get; private set; }

        private void Awake()
        {
            NearnessToGod = startingNearness;
            TemptedAway = startingTemptation;
            if (GameManager.Instance != null) GameManager.Instance.State.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null) GameManager.Instance.State.OnStateChanged -= HandleStateChanged;
        }

        /// <summary>Adjust nearness to God by a delta. Clamped to [0, 100].</summary>
        public void AdjustNearness(float delta)
        {
            NearnessToGod = Mathf.Clamp(NearnessToGod + delta, 0f, 100f);
            Persist();
        }

        /// <summary>Adjust temptation by a delta. Clamped to [0, 100].</summary>
        public void AdjustTemptation(float delta)
        {
            TemptedAway = Mathf.Clamp(TemptedAway + delta, 0f, 100f);
            Persist();
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            if (next == GameState.Ending) Persist();
        }

        private void Persist()
        {
            if (Save.SaveSystem.Instance == null) return;
            Save.SaveSystem.Instance.SetStat("morality_nearness", Mathf.RoundToInt(NearnessToGod));
            Save.SaveSystem.Instance.SetStat("morality_temptation", Mathf.RoundToInt(TemptedAway));
        }
    }
}
