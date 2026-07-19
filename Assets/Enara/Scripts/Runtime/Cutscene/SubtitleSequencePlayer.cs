using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enara.Cutscene
{
    /// <summary>
    /// Plays a sequence of subtitle lines one after another. Used for recited prayers (Psalm 50,
    /// the Jesus Prayer over a long beat) and for the Virgin Mary's "Wake up!" sequence.
    ///
    /// Author lines as a list of <see cref="SubtitleLine"/> on this component (or set them from
    /// code). Each line shows for its <see cref="SubtitleLine.duration"/> seconds before the
    /// next starts. Fires <see cref="OnSequenceFinished"/> when done.
    /// </summary>
    public sealed class SubtitleSequencePlayer : MonoBehaviour
    {
        [Serializable]
        public struct SubtitleLine
        {
            [TextArea(1, 4)] public string text;
            [Min(0.1f)] public float duration;
            [Tooltip("Optional speaker label prefixed before the text. Leave blank for none.")]
            public string speaker;
        }

        [SerializeField] private UI.Subtitles subtitles;
        [SerializeField] private List<SubtitleLine> lines = new();
        [SerializeField] private bool playOnStart = false;

        /// <summary>Raised after the last line has finished displaying.</summary>
        public event Action OnSequenceFinished;

        private Coroutine _routine;

        private void Start()
        {
            if (playOnStart) Play();
        }

        /// <summary>Begin playing the assigned sequence from the start.</summary>
        public void Play()
        {
            if (subtitles == null) subtitles = FindObjectOfType<UI.Subtitles>();
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(PlayRoutine());
        }

        /// <summary>Stop the sequence and hide the subtitle UI.</summary>
        public void Stop()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            if (subtitles != null) subtitles.Hide();
        }

        /// <summary>Replace the lines at runtime (e.g. pick a different Psalm).</summary>
        public void SetLines(IReadOnlyList<SubtitleLine> newLines)
        {
            lines.Clear();
            for (int i = 0; i < newLines.Count; i++) lines.Add(newLines[i]);
        }

        private System.Collections.IEnumerator PlayRoutine()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                string text = string.IsNullOrEmpty(line.speaker)
                    ? line.text
                    : $"{line.speaker}: {line.text}";
                if (subtitles != null) subtitles.Show(text, line.duration);
                yield return new WaitForSeconds(line.duration);
            }
            if (subtitles != null) subtitles.Hide();
            _routine = null;
            OnSequenceFinished?.Invoke();
        }
    }
}
