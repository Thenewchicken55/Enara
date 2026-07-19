using UnityEngine;
using Enara.Core;

namespace Enara.Story
{
    /// <summary>
    /// Marks a GameObject as a checkpoint. When the player enters this trigger, the current
    /// chapter is recorded in the save file so a "Continue" action can drop them back here.
    ///
    /// Use sparingly - a handful per chapter is enough.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class ChapterCheckpoint : MonoBehaviour
    {
        [SerializeField] private string checkpointId;
        [SerializeField] private string triggerTag = "Player";

        private void Reset() { var c = GetComponent<Collider>(); if (c != null) c.isTrigger = true; }
        private void Awake() { var c = GetComponent<Collider>(); if (c != null) c.isTrigger = true; }

        private void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;
            if (Save.SaveSystem.Instance == null) return;
            var chapterId = ChapterDirector.Current?.Id.Value ?? string.Empty;
            Save.SaveSystem.Instance.CurrentChapter = chapterId;
            Save.SaveSystem.Instance.SetStat($"checkpoint_{chapterId}", 1);
            Save.SaveSystem.Instance.SetFlag($"visited_{checkpointId}");
            Debug.Log($"[ChapterCheckpoint] Saved at checkpoint '{checkpointId}' (chapter '{chapterId}').");
        }
    }
}
