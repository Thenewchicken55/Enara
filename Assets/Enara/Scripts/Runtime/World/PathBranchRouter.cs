using UnityEngine;
using Enara.Core;
using Enara.SceneFlow;

namespace Enara.World
{
    /// <summary>
    /// Routes the player to one of three path chapters based on which trigger zone they entered.
    /// Lives in the Forest_Paths scene - wire each path-entrance TriggerZone's onEnter event
    /// to <see cref="ChoosePath1"/>/<see cref="ChoosePath2"/>/<see cref="ChoosePath3"/>.
    ///
    /// The actual scene transition is delegated to <see cref="ChapterDirector"/>.
    /// </summary>
    public sealed class PathBranchRouter : MonoBehaviour
    {
        [SerializeField] private ChapterDirector director;
        [SerializeField] private ChapterId path1Id = new ChapterId { Value = "Path1_OldMan" };
        [SerializeField] private ChapterId path2Id = new ChapterId { Value = "Path2_Babel" };
        [SerializeField] private ChapterId path3Id = new ChapterId { Value = "Path3_Monastery" };
        [SerializeField] private bool saveChoice = true;

        private void Reset()
        {
            if (director == null) director = FindObjectOfType<ChapterDirector>();
        }

        private void Awake()
        {
            if (director == null) director = FindObjectOfType<ChapterDirector>();
        }

        /// <summary>Load Path 1 (Old Man). Wire to a TriggerZone at Path 1's entrance.</summary>
        public void ChoosePath1() => GoTo(path1Id, "path1_chosen");

        /// <summary>Load Path 2 (Tower of Babel). Wire to a TriggerZone at Path 2's entrance.</summary>
        public void ChoosePath2() => GoTo(path2Id, "path2_chosen");

        /// <summary>Load Path 3 (Monastery). Wire to a TriggerZone at Path 3's entrance.</summary>
        public void ChoosePath3() => GoTo(path3Id, "path3_chosen");

        private void GoTo(ChapterId id, string flagToSet)
        {
            if (saveChoice && Save.SaveSystem.Instance != null) Save.SaveSystem.Instance.SetFlag(flagToSet);
            if (director != null) director.GoToChapter(id);
            else Debug.LogError($"[PathBranchRouter] No ChapterDirector in scene - cannot go to {id}.");
        }
    }
}
