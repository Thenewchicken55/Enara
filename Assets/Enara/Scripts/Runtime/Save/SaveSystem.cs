using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Enara.Save
{
    /// <summary>
    /// JSON save system. Stores a single save slot at <c>Application.persistentDataPath/Enara/save.json</c>.
    /// Tracks simple key-style flags ("took_easy_path", "talked_to_idol") and integer stats
    /// (QTEs succeeded, chapters visited). The <see cref="Enara.Dialogue.DialogueRunner"/>
    /// uses <see cref="SetFlag"/> to record branching choices; the <see cref="Enara.Core.GameManager"/>
    /// owns one of these as a singleton.
    /// </summary>
    public sealed class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private static readonly string kSaveFolder = "Enara";
        private static readonly string kSaveFile = "save.json";

        private SaveData _data = new SaveData();
        private string SavePath => Path.Combine(Application.persistentDataPath, kSaveFolder, kSaveFile);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Load();
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        // ---- Flags (booleans used for branching) ------------------------------------

        public void SetFlag(string key)
        {
            for (int i = 0; i < _data.flags.Count; i++)
                if (_data.flags[i].key == key) { _data.flags[i] = new KvpBool { key = key, value = true }; Save(); return; }
            _data.flags.Add(new KvpBool { key = key, value = true });
            Save();
        }

        public bool HasFlag(string key)
        {
            for (int i = 0; i < _data.flags.Count; i++)
                if (_data.flags[i].key == key) return _data.flags[i].value;
            return false;
        }

        public void ClearFlag(string key)
        {
            for (int i = 0; i < _data.flags.Count; i++)
                if (_data.flags[i].key == key) { _data.flags.RemoveAt(i); Save(); return; }
        }

        // ---- Integer stats -----------------------------------------------------------

        public void SetStat(string key, int value)
        {
            for (int i = 0; i < _data.stats.Count; i++)
                if (_data.stats[i].key == key) { _data.stats[i] = new KvpInt { key = key, value = value }; Save(); return; }
            _data.stats.Add(new KvpInt { key = key, value = value });
            Save();
        }

        public int GetStat(string key, int defaultValue = 0)
        {
            for (int i = 0; i < _data.stats.Count; i++)
                if (_data.stats[i].key == key) return _data.stats[i].value;
            return defaultValue;
        }

        public void IncrementStat(string key)
        {
            for (int i = 0; i < _data.stats.Count; i++)
                if (_data.stats[i].key == key) { _data.stats[i] = new KvpInt { key = key, value = _data.stats[i].value + 1 }; Save(); return; }
            _data.stats.Add(new KvpInt { key = key, value = 1 });
            Save();
        }

        // ---- Chapter progression ----------------------------------------------------

        public string CurrentChapter
        {
            get => _data.currentChapter;
            set { _data.currentChapter = value; Save(); }
        }

        /// <summary>Wipe the save back to a fresh state.</summary>
        public void Reset()
        {
            _data = new SaveData();
            Save();
        }

        // ---- Persistence -------------------------------------------------------------

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(SavePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonUtility.ToJson(_data, prettyPrint: true);
                File.WriteAllText(SavePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Failed to write save: {e.Message}");
            }
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(SavePath)) { _data = new SaveData(); return; }
                var json = File.ReadAllText(SavePath);
                _data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Failed to load save: {e.Message}");
                _data = new SaveData();
            }
        }

        public bool HasSaveFile() => File.Exists(SavePath);
    }

    /// <summary>Serialized save data. JsonUtility can't serialize Dictionary, so we use List&lt;Kvp&gt;.</summary>
    [System.Serializable]
    public sealed class SaveData
    {
        public string currentChapter = string.Empty;
        public List<KvpBool> flags = new();
        public List<KvpInt> stats = new();
    }

    [System.Serializable]
    public struct KvpBool { public string key; public bool value; }

    [System.Serializable]
    public struct KvpInt { public string key; public int value; }
}
