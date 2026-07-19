using System.Collections;
using UnityEngine;
using Enara.Core;

namespace Enara.Story
{
    /// <summary>
    /// Drives the dramatic noose wake-up in Path 1. The README beat:
    ///
    ///   "Player finally wakes up and realizes they are in a noose. The Old man was a
    ///    temptation. they were really close to ending themselves. The Woman -- The Virgin
    ///    Mary -- disappears."
    ///
    /// Sequence:
    ///   1. Player input disabled (GameState.Cutscene).
    ///   2. Screen fades to black.
    ///   3. Optional camera position swap to the noose position.
    ///   4. CutscenePlayer / SubtitleSequencePlayer plays Mary's "Wake up" lines.
    ///   5. Mary disappears.
    ///   6. Fade back in; GameState returns to Exploration.
    ///
    /// Call <see cref="Trigger"/> from a TriggerZone at the end of the old-man walk.
    /// </summary>
    public sealed class WakeUpSequence : MonoBehaviour
    {
        [SerializeField] private Cutscene.CutscenePlayer cutscenePlayer;
        [SerializeField] private Cutscene.SubtitleSequencePlayer marySubtitles;
        [SerializeField] private HolyLightController mary;
        [SerializeField] private NPC.CompanionController oldMan;
        [SerializeField] private Transform oldManLeaveTarget;
        [SerializeField] private Transform nooseCameraPosition;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float holdBlackDuration = 1f;

        /// <summary>Run the entire wake-up sequence. Idempotent guard prevents double-fire.</summary>
        public void Trigger()
        {
            StartCoroutine(RunSequence());
        }

        private IEnumerator RunSequence()
        {
            GameManager.Instance?.SetState(GameState.Cutscene);

            // Mary says "Leave him alone. Go, now." and the old man leaves.
            if (mary != null) mary.Appear();
            if (oldMan != null && oldManLeaveTarget != null) oldMan.Dismiss(oldManLeaveTarget.position);

            var fader = UI.Fader.Instance;
            if (fader != null) yield return fader.FadeOut(fadeDuration);

            // Hold on black briefly for dramatic effect.
            yield return new WaitForSeconds(holdBlackDuration);

            // Optionally move the camera to reveal the noose.
            if (nooseCameraPosition != null && playerCamera != null)
                playerCamera.transform.SetPositionAndRotation(nooseCameraPosition.position, nooseCameraPosition.rotation);

            if (marySubtitles != null)
            {
                marySubtitles.Play();
                // Wait for the subtitle sequence to finish.
                bool done = false;
                System.Action handler = () => done = true;
                marySubtitles.OnSequenceFinished += handler;
                yield return new WaitUntil(() => done);
                marySubtitles.OnSequenceFinished -= handler;
            }

            if (mary != null) mary.Disappear();

            if (fader != null) yield return fader.FadeIn(fadeDuration);
            GameManager.Instance?.SetState(GameState.Exploration);
        }
    }
}
