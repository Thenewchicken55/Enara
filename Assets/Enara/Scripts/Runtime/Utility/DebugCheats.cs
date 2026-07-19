using UnityEngine;
using Enara.Core;
using Enara.SceneFlow;

namespace Enara.Utility
{
    /// <summary>
    /// Dev-only cheat keys. Disables itself in non-development builds. Useful for testing:
    /// jump to chapter, heal player, toggle invincibility, etc.
    ///
    /// Place on the Boot scene root. Keys (override in Inspector):
    ///   F1  - log current state + position to console
    ///   F2  - heal player (skip QTE loop)
    ///   F3  - toggle player limp
    ///   F5  - advance to next chapter
    ///   F9  - wipe save
    ///   F12 - reload current scene
    /// </summary>
    public sealed class DebugCheats : MonoBehaviour
    {
        [SerializeField] private Player.PlayerController player;
        [SerializeField] private QTE.QuickTimeEventSystem qteSystem;
        [SerializeField] private ChapterDirector chapterDirector;

        private void Awake()
        {
#if !DEVELOPMENT_BUILD && UNITY_EDITOR
            enabled = true;
#else
            enabled = false;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) LogState();
            if (Input.GetKeyDown(KeyCode.F2)) HealPlayer();
            if (Input.GetKeyDown(KeyCode.F3)) ToggleLimp();
            if (Input.GetKeyDown(KeyCode.F5)) AdvanceChapter();
            if (Input.GetKeyDown(KeyCode.F9)) WipeSave();
            if (Input.GetKeyDown(KeyCode.F12)) ReloadScene();
        }

        private void LogState()
        {
            var state = GameManager.Instance != null ? GameManager.Instance.State.Current.ToString() : "null";
            var pos = player != null ? player.transform.position : Vector3.zero;
            Debug.Log($"[Debug] State={state}, PlayerPos={pos}, HasSave={Save.SaveSystem.Instance != null && Save.SaveSystem.Instance.HasSaveFile()}");
        }

        private void HealPlayer()
        {
            if (player != null) player.Limping = false;
            Debug.Log("[Debug] Player healed.");
        }

        private void ToggleLimp()
        {
            if (player != null) { player.Limping = !player.Limping; Debug.Log($"[Debug] Limping={player.Limping}"); }
        }

        private void AdvanceChapter()
        {
            if (chapterDirector != null) chapterDirector.Advance();
        }

        private void WipeSave()
        {
            if (Save.SaveSystem.Instance != null) { Save.SaveSystem.Instance.Reset(); Debug.Log("[Debug] Save wiped."); }
        }

        private void ReloadScene()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.path);
        }
    }
}
