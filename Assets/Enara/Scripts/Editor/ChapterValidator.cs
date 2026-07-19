using System.IO;
using UnityEditor;
using UnityEngine;
using Enara.Core;
using Enara.SceneFlow;

namespace Enara.Editor
{
    /// <summary>
    /// Validates that every chapter in a <see cref="ChapterDirector"/>'s list points at a
    /// scene file that actually exists on disk. Use the menu item <c>Enara > Validate
    /// Chapters</c> before a build.
    /// </summary>
    public static class ChapterValidator
    {
        [MenuItem("Enara/Validate Chapters")]
        public static void ValidateAll()
        {
            var directors = Object.FindObjectsByType<ChapterDirector>(FindObjectsSortMode.None);
            if (directors.Length == 0)
            {
                Debug.LogWarning("[ChapterValidator] No ChapterDirector found in open scenes.");
                return;
            }

            int totalErrors = 0;
            foreach (var d in directors)
            {
                var chaptersField = d.GetType().GetField("chapters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (chaptersField == null) continue;
                var list = chaptersField.GetValue(d) as System.Collections.Generic.List<ChapterDefinition>;
                if (list == null) continue;
                foreach (var chapter in list)
                {
                    if (chapter == null) { Debug.LogWarning($"[ChapterValidator] Null chapter in director on '{d.gameObject.name}'."); totalErrors++; continue; }
                    string path = chapter.ScenePath;
                    if (string.IsNullOrEmpty(path)) { Debug.LogWarning($"[ChapterValidator] Chapter '{chapter.Id}' has no scene path."); totalErrors++; continue; }
                    if (!File.Exists(path) && !AssetDatabase.LoadAssetAtPath<SceneAsset>(path))
                    {
                        Debug.LogWarning($"[ChapterValidator] Chapter '{chapter.Id}' references scene '{path}' but it doesn't exist.");
                        totalErrors++;
                    }
                    else
                    {
                        Debug.Log($"[ChapterValidator] OK: chapter '{chapter.Id}' -> '{path}'.");
                    }
                }
            }

            if (totalErrors == 0) Debug.Log("[ChapterValidator] All chapters valid.");
            else Debug.LogError($"[ChapterValidator] {totalErrors} problem(s) found. Fix before building.");
        }
    }
}
