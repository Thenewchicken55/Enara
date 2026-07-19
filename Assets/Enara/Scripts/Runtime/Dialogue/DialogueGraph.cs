using System.Collections.Generic;
using UnityEngine;

namespace Enara.Dialogue
{
    /// <summary>
    /// One line of dialogue spoken by a character. A node optionally ends in a set of choices
    /// that the player can pick; if it has no choices, it advances to <see cref="NextNodeId"/>
    /// (or ends the conversation if NextNodeId is empty).
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueNode", menuName = "Enara/Dialogue Node", order = 1)]
    public sealed class DialogueNode : ScriptableObject
    {
        [SerializeField] private string nodeId;
        [SerializeField] private string speakerName = string.Empty;
        [SerializeField, TextArea(2, 8)] private string text = string.Empty;
        [SerializeField, Tooltip("Optional VO clip. If set, AudioManager plays it when the node runs.")]
        private AudioClip voiceOver;
        [SerializeField] private float autoAdvanceDelay = 3.5f;
        [SerializeField, Tooltip("ID of the next node. Leave empty to end the conversation.")]
        private string nextNodeId = string.Empty;
        [SerializeField] private List<DialogueChoice> choices = new();

        public string NodeId => nodeId;
        public string SpeakerName => speakerName;
        public string Text => text;
        public AudioClip VoiceOver => voiceOver;
        public float AutoAdvanceDelay => autoAdvanceDelay;
        public string NextNodeId => nextNodeId;
        public IReadOnlyList<DialogueChoice> Choices => choices;
    }

    /// <summary>A pickable choice attached to a <see cref="DialogueNode"/>.</summary>
    [System.Serializable]
    public struct DialogueChoice
    {
        [SerializeField] private string choiceId;
        [SerializeField, TextArea(1, 3)] private string label;
        [SerializeField, Tooltip("Node to jump to if the player picks this. Leave empty to end conversation.")]
        private string targetNodeId;
        [SerializeField, Tooltip("If set, the GameManager flags this choice (used for branching endings).")]
        private string flagToSet;

        public string ChoiceId => choiceId;
        public string Label => label;
        public string TargetNodeId => targetNodeId;
        public string FlagToSet => flagToSet;
    }

    /// <summary>
    /// A whole conversation. Holds the entry node and a flat list of nodes by ID so the
    /// <see cref="DialogueRunner"/> can look up any node in O(1).
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueGraph", menuName = "Enara/Dialogue Graph", order = 0)]
    public sealed class DialogueGraph : ScriptableObject
    {
        [SerializeField] private string entryNodeId;
        [SerializeField] private List<DialogueNode> nodes = new();

        public string EntryNodeId => entryNodeId;

        /// <summary>Look up a node by its ID. Returns null if not found.</summary>
        public DialogueNode Find(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return null;
            if (nodes == null) return null;
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i] != null && nodes[i].NodeId == nodeId) return nodes[i];
            return null;
        }
    }
}
