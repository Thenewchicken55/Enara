using UnityEngine;
using TMPro;

namespace Enara.UI
{
    /// <summary>
    /// Tiny heads-up display. Optional - shows current chapter title (top-left) and an optional
    /// objective line (top-right). Wired in the Inspector; can be left unused if not needed.
    /// </summary>
    public sealed class HUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text chapterLabel;
        [SerializeField] private TMP_Text objectiveLabel;

        public void SetChapter(string text) { if (chapterLabel != null) chapterLabel.text = text; }
        public void SetObjective(string text) { if (objectiveLabel != null) objectiveLabel.text = text; }
        public void ClearObjective() { if (objectiveLabel != null) objectiveLabel.text = string.Empty; }
    }
}
