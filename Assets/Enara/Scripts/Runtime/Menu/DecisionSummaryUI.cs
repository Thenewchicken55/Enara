using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Enara.Menu
{
    /// <summary>
    /// At the end of the game, shows the player a summary of the choices they made. Reads
    /// flags from the <see cref="Save.SaveSystem"/> and displays a description line for each
    /// known flag.
    ///
    /// Author the <see cref="knownFlags"/> list once, mapping each save flag to a human-readable
    /// description. The screen shows only the flags that are actually set.
    /// </summary>
    public sealed class DecisionSummaryUI : MonoBehaviour
    {
        [System.Serializable]
        public struct DecisionEntry
        {
            public string flagKey;
            [TextArea(1, 4)] public string descriptionIfSet;
        }

        [SerializeField] private GameObject root;
        [SerializeField] private Transform entryContainer;
        [SerializeField] private GameObject entryPrefab; // TMP_Text prefab, one per line
        [SerializeField] private List<DecisionEntry> knownFlags = new();

        public void Show()
        {
            if (root != null) root.SetActive(true);
            ClearExisting();
            Populate();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        private void ClearExisting()
        {
            if (entryContainer == null) return;
            for (int i = entryContainer.childCount - 1; i >= 0; i--)
                Destroy(entryContainer.GetChild(i).gameObject);
        }

        private void Populate()
        {
            if (entryContainer == null || entryPrefab == null) return;
            var save = Save.SaveSystem.Instance;
            foreach (var entry in knownFlags)
            {
                if (save == null || !save.HasFlag(entry.flagKey)) continue;
                var go = Instantiate(entryPrefab, entryContainer);
                if (go.TryGetComponent<TMP_Text>(out var text)) text.text = entry.descriptionIfSet;
                else
                {
                    var child = go.GetComponentInChildren<TMP_Text>();
                    if (child != null) child.text = entry.descriptionIfSet;
                }
            }
        }
    }
}
