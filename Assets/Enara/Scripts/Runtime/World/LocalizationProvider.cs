using System.Collections.Generic;
using UnityEngine;

namespace Enara.World
{
    /// <summary>
    /// README Path 2: "I need voice actors here, in different languages." Instead of recording
    /// multiple VO takes, this provider lets you swap the active language at runtime; the
    /// <see cref="Enara.Audio.AudioManager"/> and <see cref="Enara.UI.Subtitles"/> can use it
    /// to pick the right clip / text.
    ///
    /// Author languages as ScriptableObject instances (one per language) with a dictionary-like
    /// lookup of string -> localized string. Keys are arbitrary ("babel_leader_intro").
    /// </summary>
    [CreateAssetMenu(fileName = "LanguagePack", menuName = "Enara/Language Pack", order = 20)]
    public sealed class LocalizationProvider : ScriptableObject
    {
        [SerializeField] private string languageId = "en";
        [SerializeField] private List<Entry> entries = new();

        public string LanguageId => languageId;

        /// <summary>Look up a localized string. Returns the key itself if missing.</summary>
        public string Get(string key)
        {
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].key == key) return entries[i].value;
            return key;
        }

        /// <summary>Try to look up a localized string.</summary>
        public bool TryGet(string key, out string value)
        {
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].key == key) { value = entries[i].value; return true; }
            value = null;
            return false;
        }

        [System.Serializable]
        public struct Entry
        {
            public string key;
            [TextArea(1, 4)] public string value;
        }
    }
}
