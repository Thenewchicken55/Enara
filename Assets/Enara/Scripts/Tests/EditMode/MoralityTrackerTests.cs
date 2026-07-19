using NUnit.Framework;
using UnityEngine;
using Enara.Story;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for <see cref="MoralityTracker"/>. Focuses on the clamping logic and
    /// the persistence of values to the save system. The singleton coupling with
    /// <see cref="Core.GameManager"/> is null-checked in production code so it doesn't interfere
    /// with these tests.
    /// </summary>
    [TestFixture]
    public class MoralityTrackerTests
    {
        private GameObject _gameObject;
        private MoralityTracker _tracker;
        private Save.SaveSystem _saveSystem;
        private GameObject _saveGo;

        [SetUp]
        public void SetUp()
        {
            // Destroy any leftover singletons.
            if (Save.SaveSystem.Instance != null)
            {
                Object.DestroyImmediate(Save.SaveSystem.Instance.gameObject);
            }

            // Create a SaveSystem first so the tracker has somewhere to persist.
            _saveGo = new GameObject("TestSaveSystem");
            _saveSystem = _saveGo.AddComponent<Save.SaveSystem>();
            _saveSystem.Reset();

            _gameObject = new GameObject("TestMorality");
            _tracker = _gameObject.AddComponent<MoralityTracker>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameObject != null) Object.DestroyImmediate(_gameObject);
            if (_saveGo != null) Object.DestroyImmediate(_saveGo);
            // Clean up the save file.
            var savePath = System.IO.Path.Combine(Application.persistentDataPath, "Enara", "save.json");
            if (System.IO.File.Exists(savePath)) System.IO.File.Delete(savePath);
        }

        [Test]
        public void Awake_InitializesToStartingValues()
        {
            // Default values in the inspector: startingNearness=20, startingTemptation=10.
            // When AddComponent is called in EditMode, Awake runs with the serialized defaults.
            Assert.AreEqual(20f, _tracker.NearnessToGod);
            Assert.AreEqual(10f, _tracker.TemptedAway);
        }

        [Test]
        public void AdjustNearness_PositiveDelta_IncreasesValue()
        {
            _tracker.AdjustNearness(15f);

            Assert.AreEqual(35f, _tracker.NearnessToGod, 0.001f);
        }

        [Test]
        public void AdjustNearness_NegativeDelta_DecreasesValue()
        {
            _tracker.AdjustNearness(-5f);

            Assert.AreEqual(15f, _tracker.NearnessToGod, 0.001f);
        }

        [Test]
        public void AdjustNearness_ClampsAtZero()
        {
            _tracker.AdjustNearness(-100f);

            Assert.AreEqual(0f, _tracker.NearnessToGod, 0.001f, "Nearness should clamp at 0.");
        }

        [Test]
        public void AdjustNearness_ClampsAtHundred()
        {
            _tracker.AdjustNearness(200f);

            Assert.AreEqual(100f, _tracker.NearnessToGod, 0.001f, "Nearness should clamp at 100.");
        }

        [Test]
        public void AdjustTemptation_PositiveDelta_IncreasesValue()
        {
            _tracker.AdjustTemptation(20f);

            Assert.AreEqual(30f, _tracker.TemptedAway, 0.001f);
        }

        [Test]
        public void AdjustTemptation_ClampsAtZero()
        {
            _tracker.AdjustTemptation(-100f);

            Assert.AreEqual(0f, _tracker.TemptedAway, 0.001f);
        }

        [Test]
        public void AdjustTemptation_ClampsAtHundred()
        {
            _tracker.AdjustTemptation(150f);

            Assert.AreEqual(100f, _tracker.TemptedAway, 0.001f);
        }

        [Test]
        public void MultipleAdjustments_AccumulateCorrectly()
        {
            _tracker.AdjustNearness(10f);
            _tracker.AdjustNearness(5f);
            _tracker.AdjustNearness(-3f);

            Assert.AreEqual(32f, _tracker.NearnessToGod, 0.001f);
        }

        [Test]
        public void AdjustNearness_PersistsToSaveSystem()
        {
            _tracker.AdjustNearness(25f);

            int saved = _saveSystem.GetStat("morality_nearness");

            Assert.AreEqual(45, saved); // 20 starting + 25 = 45, rounded to int.
        }

        [Test]
        public void AdjustTemptation_PersistsToSaveSystem()
        {
            _tracker.AdjustTemptation(15f);

            int saved = _saveSystem.GetStat("morality_temptation");

            Assert.AreEqual(25, saved); // 10 starting + 15 = 25.
        }

        [Test]
        public void AdjustNearness_ToMaxValue_Persists100()
        {
            _tracker.AdjustNearness(500f);

            int saved = _saveSystem.GetStat("morality_nearness");

            Assert.AreEqual(100, saved);
        }

        [Test]
        public void NearnessAndTemptation_AreIndependent()
        {
            _tracker.AdjustNearness(50f);
            _tracker.AdjustTemptation(5f);

            Assert.AreEqual(70f, _tracker.NearnessToGod, 0.001f);
            Assert.AreEqual(15f, _tracker.TemptedAway, 0.001f);
        }

        [Test]
        public void AdjustNearness_WithZeroDelta_DoesNothing()
        {
            float before = _tracker.NearnessToGod;

            _tracker.AdjustNearness(0f);

            Assert.AreEqual(before, _tracker.NearnessToGod);
        }
    }
}
