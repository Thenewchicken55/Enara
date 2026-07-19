using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enara.Choice
{
    using Dialogue;

    /// <summary>
    /// API surface that the <see cref="DialogueRunner"/> uses to present choices. The concrete
    /// implementation <see cref="ChoicePresenter"/> renders the choices to UGUI; tests or other
    /// consumers can implement this interface directly.
    /// </summary>
    public interface IChoiceView
    {
        void Show(IReadOnlyList<DialogueChoice> choices, Action<DialogueChoice> onChosen);
        void Hide();
    }

    /// <summary>
    /// Renders the choices attached to the current dialogue node as buttons. Designed to be
    /// wired in the Inspector: a prefab button and a parent layout group.
    /// </summary>
    public sealed class ChoicePresenter : MonoBehaviour, IChoiceView
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;

        private readonly List<GameObject> _spawned = new();
        private Action<DialogueChoice> _onChosen;

        private void Reset() { if (root == null) root = gameObject; }
        private void Awake() { if (root == null) root = gameObject; Hide(); }

        public void Show(IReadOnlyList<DialogueChoice> choices, Action<DialogueChoice> onChosen)
        {
            _onChosen = onChosen;
            ClearSpawned();
            if (root != null) root.SetActive(true);

            if (choices == null || choices.Count == 0) return;
            if (buttonPrefab == null || buttonContainer == null)
            {
                Debug.LogWarning("[ChoicePresenter] buttonPrefab or buttonContainer not assigned.");
                return;
            }

            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                var go = Instantiate(buttonPrefab, buttonContainer);
                _spawned.Add(go);
                if (go.TryGetComponent<UnityEngine.UI.Button>(out var btn))
                {
                    var label = go.GetComponentInChildren<TMPro.TMP_Text>();
                    if (label != null) label.text = choice.Label;
                    btn.onClick.AddListener(() => _onChosen?.Invoke(choice));
                }
            }
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
            ClearSpawned();
        }

        private void ClearSpawned()
        {
            foreach (var go in _spawned)
                if (go != null) Destroy(go);
            _spawned.Clear();
        }
    }
}
