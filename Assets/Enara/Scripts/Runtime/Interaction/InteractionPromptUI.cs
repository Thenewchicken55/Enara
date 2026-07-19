using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enara.Interaction
{
    /// <summary>
    /// Simple prompt UI: shows a text label like "Press E to talk" when the player is looking at
    /// an interactable. Uses TextMeshPro (UGUI). The Interactor drives it via <see cref="Show"/>
    /// / <see cref="Hide"/>.
    ///
    /// Wiring: place a Canvas in the scene, add a TextMeshProUGUI child, drag both into this
    /// component. The Interactor finds this component automatically.
    /// </summary>
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text label;

        private void Reset()
        {
            if (root == null) root = gameObject;
        }

        private void Awake()
        {
            if (root == null) root = gameObject;
            Hide();
        }

        /// <summary>Display a prompt and enable the root.</summary>
        public void Show(string text)
        {
            if (label != null) label.text = text;
            if (root != null) root.SetActive(true);
        }

        /// <summary>Hide the prompt.</summary>
        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }
}
