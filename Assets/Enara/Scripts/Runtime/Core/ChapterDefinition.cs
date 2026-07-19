using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enara.Core
{
    /// <summary>
    /// Type-safe string IDs for chapters. Stored as plain strings so they survive
    /// serialization in ScriptableObjects, but compared via the typed <see cref="ChapterId"/>
    /// struct to avoid typos in code.
    /// </summary>
    [Serializable]
    public struct ChapterId : IEquatable<ChapterId>
    {
        [SerializeField] private string value;
        public string Value => value;
        public ChapterId(string v) => value = v;
        public bool Equals(ChapterId other) => value == other.value;
        public override bool Equals(object obj) => obj is ChapterId other && Equals(other);
        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
        public override string ToString() => value ?? string.Empty;
        public static bool operator ==(ChapterId a, ChapterId b) => a.Equals(b);
        public static bool operator !=(ChapterId a, ChapterId b) => !a.Equals(b);
        public static implicit operator string(ChapterId c) => c.value;
    }

    /// <summary>
    /// Definition of a chapter - a discrete chunk of the game. Each maps roughly to one
    /// README beat: Intro_Drive, Crash_Limp, Path1_OldMan, Path2_Babel, Path3_Monastery, Ending.
    ///
    /// Create instances as ScriptableObject assets under Assets/Enara/Data/Chapters/.
    /// </summary>
    [CreateAssetMenu(fileName = "NewChapter", menuName = "Enara/Chapter", order = 0)]
    public sealed class ChapterDefinition : ScriptableObject
    {
        [SerializeField] private ChapterId id;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField, Tooltip("Scene asset (or path under Assets/Enara/Scenes/) to load.")]
        private string scenePath = string.Empty;
        [SerializeField, TextArea(3, 12)]
        [Tooltip("Designer notes - what should happen in this chapter. Not shown to players.")]
        private string designerNotes = string.Empty;

        public ChapterId Id => id;
        public string DisplayName => displayName;
        public string ScenePath => scenePath;
        public string DesignerNotes => designerNotes;
    }
}
