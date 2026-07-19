using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Enara.Core;

namespace Enara.Cutscene
{
    /// <summary>
    /// Wrapper around Unity's <see cref="PlayableDirector"/> that:
    /// - Puts the game in <see cref="GameState.Cutscene"/> while playing (so player input is locked).
    /// - Optionally fades in/out via <see cref="UI.Fader"/>.
    /// - Exposes an <see cref="OnFinished"/> event for chaining the next beat.
    ///
    /// Use this for the driving intro, the crash, the noose wake-up, and any other Timeline-driven
    /// sequence. Author the Timeline as a <see cref="PlayableAsset"/> and drag it in.
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public sealed class CutscenePlayer : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private bool fadeOnStart = true;
        [SerializeField] private bool fadeOnEnd = true;
        [SerializeField] private float fadeDuration = 0.8f;
        [SerializeField] private bool lockPlayerInput = true;

        /// <summary>Raised when the timeline finishes (after the end fade, if any).</summary>
        public event Action OnFinished;

        private Coroutine _routine;
        private bool _isPlaying;

        /// <summary>True while a cutscene is currently playing.</summary>
        public bool IsPlaying => _isPlaying;

        private void Reset()
        {
            if (director == null) director = GetComponent<PlayableDirector>();
        }

        private void Awake()
        {
            if (director == null) director = GetComponent<PlayableDirector>();
            if (director != null) director.stopped += _ => HandleDirectorStopped();
        }

        /// <summary>Begin playing the assigned timeline.</summary>
        public void Play()
        {
            if (director == null) { Debug.LogWarning("[CutscenePlayer] No PlayableDirector assigned."); return; }
            if (_isPlaying) return;
            _isPlaying = true;
            if (lockPlayerInput) GameManager.Instance?.SetState(GameState.Cutscene);
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(PlayRoutine());
        }

        /// <summary>Stop the timeline immediately and clean up state.</summary>
        public void Stop()
        {
            if (director != null) director.Stop();
            Cleanup();
        }

        private IEnumerator PlayRoutine()
        {
            var fader = UI.Fader.Instance;
            if (fadeOnStart && fader != null) yield return fader.FadeOut(fadeDuration);

            director.Play();
            // Wait for the timeline to finish.
            while (director.state == PlayState.Playing) yield return null;

            if (fadeOnEnd && fader != null) yield return fader.FadeIn(fadeDuration);
            Cleanup();
        }

        private void HandleDirectorStopped()
        {
            // The director stopped (either naturally or via Stop()). Cleanup will fire the event.
            if (_isPlaying && _routine == null) Cleanup();
        }

        private void Cleanup()
        {
            _isPlaying = false;
            _routine = null;
            if (lockPlayerInput) GameManager.Instance?.SetState(GameState.Exploration);
            OnFinished?.Invoke();
        }
    }
}
