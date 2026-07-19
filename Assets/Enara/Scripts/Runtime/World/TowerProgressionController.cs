using System.Collections.Generic;
using UnityEngine;

namespace Enara.World
{
    /// <summary>
    /// Path 2 (Tower of Babel) - shows the tower at progressively taller stages as the player
    /// ascends the mountain. Each stage is a GameObject that gets activated; previous stages
    /// can stay or be replaced.
    ///
    /// Place a <see cref="TriggerZone"/> at each stage threshold and wire its onEnter to
    /// <see cref="ShowStage(int)"/> via a small inspector event.
    /// </summary>
    public sealed class TowerProgressionController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> stages = new();
        [SerializeField] private bool hidePreviousStages = false;
        [SerializeField] private int initialStage = 0;

        private int _currentStage = -1;

        private void Start() { ShowStage(initialStage); }

        /// <summary>Activate stage <paramref name="index"/> (0-based). Hides others if configured.</summary>
        public void ShowStage(int index)
        {
            if (index < 0 || index >= stages.Count) return;
            if (index == _currentStage) return;
            _currentStage = index;
            for (int i = 0; i < stages.Count; i++)
            {
                if (stages[i] == null) continue;
                bool active = hidePreviousStages ? (i == index) : (i <= index);
                stages[i].SetActive(active);
            }
        }

        /// <summary>Advance to the next stage. No-op if at the last stage.</summary>
        public void AdvanceStage()
        {
            if (_currentStage + 1 < stages.Count) ShowStage(_currentStage + 1);
        }
    }
}
