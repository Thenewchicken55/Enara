using System.Collections.Generic;
using UnityEngine;

namespace Enara.Story
{
    /// <summary>
    /// A list of scripture verses (Psalm 50, the Jesus Prayer, etc.) plus a method to play
    /// them through the <see cref="Cutscene.SubtitleSequencePlayer"/>. Used by Path 3:
    /// "interacting with the icon, the player says psalm 50".
    ///
    /// Create instances as ScriptableObjects under Assets/Enara/Data/Scripture/. Drag them
    /// onto a small component that calls <see cref="Recite"/> when the player interacts.
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptureRecitation", menuName = "Enara/Scripture Recitation", order = 10)]
    public sealed class ScriptureReciter : ScriptableObject
    {
        [SerializeField] private string recitationId = "psalm_50";
        [SerializeField] private string speaker = "Player";
        [SerializeField, Tooltip("Verses in order. Each becomes one subtitle line.")]
        private List<string> verses = new();
        [SerializeField] private float perVerseDuration = 3.5f;

        public string RecitationId => recitationId;
        public string Speaker => speaker;
        public IReadOnlyList<string> Verses => verses;
        public float PerVerseDuration => perVerseDuration;

        /// <summary>Recite by populating a <see cref="Cutscene.SubtitleSequencePlayer"/> and playing it.</summary>
        public void Recite(Cutscene.SubtitleSequencePlayer player)
        {
            if (player == null) { Debug.LogWarning("[ScriptureReciter] No SubtitleSequencePlayer."); return; }
            var lines = new List<Cutscene.SubtitleSequencePlayer.SubtitleLine>();
            foreach (var v in verses)
                lines.Add(new Cutscene.SubtitleSequencePlayer.SubtitleLine
                {
                    text = v,
                    duration = perVerseDuration,
                    speaker = speaker
                });
            player.SetLines(lines);
            player.Play();
        }
    }
}
