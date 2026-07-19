using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enara.QTE
{
    /// <summary>
    /// UI for an in-progress QTE. Shows the prompt word and a shrinking bar / timer ring so the
    /// player has visual feedback for how long they have left. Driven by <see cref="QuickTimeEventSystem"/>.
    ///
    /// Wiring: place a Canvas in the scene, add a root GameObject with a TMP_Text (the word)
    /// and an Image (the timer bar). Drag them into this component.
    /// </summary>
    public sealed class QteUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text promptLabel;
        [SerializeField] private Image timerFill;
        [SerializeField] private TMP_Text keyHintLabel;

        private void Reset() { if (root == null) root = gameObject; }
        private void Awake() { if (root == null) root = gameObject; Hide(); }

        public void Show(QteState state)
        {
            if (root != null) root.SetActive(true);
            if (promptLabel != null) promptLabel.text = state.PromptText;
            if (keyHintLabel != null) keyHintLabel.text = state.Expected.ToString();
            if (timerFill != null) timerFill.fillAmount = state.NormalizedProgress;
        }

        /// <summary>Called every frame while a QTE is active so the timer can shrink.</summary>
        public void UpdateProgress(float normalized)
        {
            if (timerFill != null) timerFill.fillAmount = Mathf.Clamp01(normalized);
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }
}
