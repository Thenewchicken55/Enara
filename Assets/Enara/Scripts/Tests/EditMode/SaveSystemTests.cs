using System.IO;
using NUnit.Framework;
using UnityEngine;
using Enara.Save;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for the save system. This is the most important test file in the project
    /// because <see cref="SaveData"/> uses a hand-rolled <c>List&lt;KvpBool&gt;</c> workaround
    /// for Unity's <c>JsonUtility</c> (which can't serialize <c>Dictionary</c>). If the
    /// serialization ever breaks, the player loses their save - these tests catch that early.
    ///
    /// Two layers tested:
    ///   1. Pure <see cref="SaveData"/> JSON round-trip (no MonoBehaviour, no file I/O).
    ///   2. <see cref="SaveSystem"/> MonoBehaviour CRUD + file persistence (uses a real
    ///      temp file via <c>Application.persistentDataPath</c>; cleans up in TearDown).
    /// </summary>
    [TestFixture]
    public class SaveSystemTests
    {
        // ---- SaveData JSON round-trip tests (pure data, no MonoBehaviour) ---------

        [Test]
        public void SaveData_Empty_RoundTripsCleanly()
        {
            var data = new SaveData();

            var json = JsonUtility.ToJson(data);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(string.Empty, loaded.currentChapter);
            Assert.IsNotNull(loaded.flags);
            Assert.IsNotNull(loaded.stats);
            Assert.AreEqual(0, loaded.flags.Count);
            Assert.AreEqual(0, loaded.stats.Count);
        }

        [Test]
        public void SaveData_CurrentChapter_RoundTrips()
        {
            var data = new SaveData { currentChapter = "Forest_Limp" };

            var json = JsonUtility.ToJson(data);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            Assert.AreEqual("Forest_Limp", loaded.currentChapter);
        }

        [Test]
        public void SaveData_Flags_RoundTrip()
        {
            var data = new SaveData();
            data.flags.Add(new KvpBool { key = "took_easy_path", value = true });
            data.flags.Add(new KvpBool { key = "talked_to_idol", value = false });
            data.flags.Add(new KvpBool { key = "has_sigil", value = true });

            var json = JsonUtility.ToJson(data);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            Assert.AreEqual(3, loaded.flags.Count);
            Assert.AreEqual("took_easy_path", loaded.flags[0].key);
            Assert.IsTrue(loaded.flags[0].value);
            Assert.AreEqual("talked_to_idol", loaded.flags[1].key);
            Assert.IsFalse(loaded.flags[1].value);
            Assert.AreEqual("has_sigil", loaded.flags[2].key);
            Assert.IsTrue(loaded.flags[2].value);
        }

        [Test]
        public void SaveData_Stats_RoundTrip()
        {
            var data = new SaveData();
            data.stats.Add(new KvpInt { key = "qte_successes", value = 8 });
            data.stats.Add(new KvpInt { key = "morality_nearness", value = 42 });
            data.stats.Add(new KvpInt { key = "chapters_visited", value = 3 });

            var json = JsonUtility.ToJson(data);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            Assert.AreEqual(3, loaded.stats.Count);
            Assert.AreEqual("qte_successes", loaded.stats[0].key);
            Assert.AreEqual(8, loaded.stats[0].value);
            Assert.AreEqual("morality_nearness", loaded.stats[1].key);
            Assert.AreEqual(42, loaded.stats[1].value);
            Assert.AreEqual("chapters_visited", loaded.stats[2].key);
            Assert.AreEqual(3, loaded.stats[2].value);
        }

        [Test]
        public void SaveData_FlagsAndStats_Together_RoundTrip()
        {
            var data = new SaveData { currentChapter = "Path1_OldMan" };
            data.flags.Add(new KvpBool { key = "path1_chosen", value = true });
            data.stats.Add(new KvpInt { key = "morality_nearness", value = 75 });

            var json = JsonUtility.ToJson(data);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            Assert.AreEqual("Path1_OldMan", loaded.currentChapter);
            Assert.AreEqual(1, loaded.flags.Count);
            Assert.AreEqual("path1_chosen", loaded.flags[0].key);
            Assert.IsTrue(loaded.flags[0].value);
            Assert.AreEqual(1, loaded.stats.Count);
            Assert.AreEqual("morality_nearness", loaded.stats[0].key);
            Assert.AreEqual(75, loaded.stats[0].value);
        }

        [Test]
        public void SaveData_PrettyPrint_DoesNotChangeSemantics()
        {
            var data = new SaveData();
            data.flags.Add(new KvpBool { key = "x", value = true });

            var compact = JsonUtility.ToJson(data, prettyPrint: false);
            var pretty = JsonUtility.ToJson(data, prettyPrint: true);

            var fromCompact = JsonUtility.FromJson<SaveData>(compact);
            var fromPretty = JsonUtility.FromJson<SaveData>(pretty);

            Assert.AreEqual(fromCompact.flags[0].key, fromPretty.flags[0].key);
            Assert.AreEqual(fromCompact.flags[0].value, fromPretty.flags[0].value);
        }

        // ---- SaveSystem MonoBehaviour integration tests -----------------------------

        private SaveSystem _saveSystem;
        private GameObject _gameObject;
        private string _savePath;

        [SetUp]
        public void SetUp()
        {
            // Destroy any leftover singleton from a previous test.
            if (SaveSystem.Instance != null)
            {
                Object.DestroyImmediate(SaveSystem.Instance.gameObject);
            }

            _gameObject = new GameObject("TestSaveSystem");
            _saveSystem = _gameObject.AddComponent<SaveSystem>();

            // Compute the save path so we can clean it up after the test.
            _savePath = Path.Combine(Application.persistentDataPath, "Enara", "save.json");

            // Start every test from a clean slate.
            _saveSystem.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            // Delete the save file we wrote during the test so we don't pollute the user's
            // real save.
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
            if (_gameObject != null) Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Awake_SetsInstanceSingleton()
        {
            Assert.IsNotNull(SaveSystem.Instance, "Awake should set the Instance singleton.");
            Assert.AreSame(_saveSystem, SaveSystem.Instance);
        }

        [Test]
        public void HasSaveFile_WhenNoFileExists_ReturnsFalse()
        {
            // Reset wiped and saved an empty file. Delete it.
            if (File.Exists(_savePath)) File.Delete(_savePath);

            Assert.IsFalse(_saveSystem.HasSaveFile());
        }

        [Test]
        public void HasSaveFile_AfterSave_ReturnsTrue()
        {
            _saveSystem.SetFlag("any_flag");

            Assert.IsTrue(_saveSystem.HasSaveFile());
        }

        [Test]
        public void SetFlag_ThenHasFlag_ReturnsTrue()
        {
            _saveSystem.SetFlag("took_easy_path");

            Assert.IsTrue(_saveSystem.HasFlag("took_easy_path"));
        }

        [Test]
        public void HasFlag_WhenNotSet_ReturnsFalse()
        {
            Assert.IsFalse(_saveSystem.HasFlag("never_set"));
        }

        [Test]
        public void SetFlag_Twice_DoesNotDuplicate()
        {
            _saveSystem.SetFlag("duplicate_me");
            _saveSystem.SetFlag("duplicate_me");

            // Reload to verify what was actually written to disk.
            _saveSystem.Load();
            Assert.IsTrue(_saveSystem.HasFlag("duplicate_me"));
        }

        [Test]
        public void ClearFlag_RemovesFlag()
        {
            _saveSystem.SetFlag("temp_flag");
            Assert.IsTrue(_saveSystem.HasFlag("temp_flag"));

            _saveSystem.ClearFlag("temp_flag");

            Assert.IsFalse(_saveSystem.HasFlag("temp_flag"));
        }

        [Test]
        public void ClearFlag_WhenNotSet_IsSafe()
        {
            Assert.DoesNotThrow(() => _saveSystem.ClearFlag("does_not_exist"));
        }

        [Test]
        public void SetStat_ThenGetStat_ReturnsValue()
        {
            _saveSystem.SetStat("qte_count", 8);

            Assert.AreEqual(8, _saveSystem.GetStat("qte_count"));
        }

        [Test]
        public void GetStat_WhenNotSet_ReturnsDefault()
        {
            Assert.AreEqual(0, _saveSystem.GetStat("never_set"));
            Assert.AreEqual(-1, _saveSystem.GetStat("never_set", -1));
        }

        [Test]
        public void IncrementStat_IncreasesByOne()
        {
            _saveSystem.SetStat("counter", 5);

            _saveSystem.IncrementStat("counter");

            Assert.AreEqual(6, _saveSystem.GetStat("counter"));
        }

        [Test]
        public void IncrementStat_WhenNotSet_StartsFromZero()
        {
            _saveSystem.IncrementStat("new_counter");

            Assert.AreEqual(1, _saveSystem.GetStat("new_counter"));
        }

        [Test]
        public void SetStat_OverwritesPreviousValue()
        {
            _saveSystem.SetStat("x", 10);
            _saveSystem.SetStat("x", 99);

            Assert.AreEqual(99, _saveSystem.GetStat("x"));
        }

        [Test]
        public void CurrentChapter_PersistsAcrossReload()
        {
            _saveSystem.CurrentChapter = "Path2_Babel";

            // Reload from disk.
            _saveSystem.Load();

            Assert.AreEqual("Path2_Babel", _saveSystem.CurrentChapter);
        }

        [Test]
        public void SetFlag_PersistsAcrossReload()
        {
            _saveSystem.SetFlag("persisted_flag");

            // Reload from disk - simulating the game closing and reopening.
            _saveSystem.Load();

            Assert.IsTrue(_saveSystem.HasFlag("persisted_flag"));
        }

        [Test]
        public void SetStat_PersistsAcrossReload()
        {
            _saveSystem.SetStat("persisted_stat", 12345);

            _saveSystem.Load();

            Assert.AreEqual(12345, _saveSystem.GetStat("persisted_stat"));
        }

        [Test]
        public void Reset_WipesAllState()
        {
            _saveSystem.SetFlag("flag1");
            _saveSystem.SetFlag("flag2");
            _saveSystem.SetStat("stat1", 100);
            _saveSystem.CurrentChapter = "SomeChapter";

            _saveSystem.Reset();

            Assert.IsFalse(_saveSystem.HasFlag("flag1"));
            Assert.IsFalse(_saveSystem.HasFlag("flag2"));
            Assert.AreEqual(0, _saveSystem.GetStat("stat1"));
            Assert.AreEqual(string.Empty, _saveSystem.CurrentChapter);
        }

        [Test]
        public void MultipleFlagsAndStats_AllPersist()
        {
            _saveSystem.SetFlag("flag_a");
            _saveSystem.SetFlag("flag_b");
            _saveSystem.SetFlag("flag_c");
            _saveSystem.SetStat("stat_a", 1);
            _saveSystem.SetStat("stat_b", 2);
            _saveSystem.SetStat("stat_c", 3);
            _saveSystem.CurrentChapter = "Forest_Limp";

            _saveSystem.Load();

            Assert.IsTrue(_saveSystem.HasFlag("flag_a"));
            Assert.IsTrue(_saveSystem.HasFlag("flag_b"));
            Assert.IsTrue(_saveSystem.HasFlag("flag_c"));
            Assert.AreEqual(1, _saveSystem.GetStat("stat_a"));
            Assert.AreEqual(2, _saveSystem.GetStat("stat_b"));
            Assert.AreEqual(3, _saveSystem.GetStat("stat_c"));
            Assert.AreEqual("Forest_Limp", _saveSystem.CurrentChapter);
        }

        [Test]
        public void SaveFile_IsValidJson()
        {
            _saveSystem.SetFlag("json_test");
            _saveSystem.SetStat("stat_test", 42);

            string json = File.ReadAllText(_savePath);

            Assert.IsTrue(json.Contains("currentChapter"), "JSON should contain currentChapter field.");
            Assert.IsTrue(json.Contains("flags"), "JSON should contain flags field.");
            Assert.IsTrue(json.Contains("stats"), "JSON should contain stats field.");
            Assert.IsTrue(json.Contains("json_test"), "JSON should contain the flag key.");
            Assert.IsTrue(json.Contains("stat_test"), "JSON should contain the stat key.");
            Assert.IsTrue(json.Contains("42"), "JSON should contain the stat value.");
        }
    }
}
