using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Enara.Dialogue;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for <see cref="DialogueGraph"/>. Tests the lookup logic without needing
    /// the runtime <see cref="DialogueRunner"/> to actually play anything.
    /// </summary>
    [TestFixture]
    public class DialogueGraphTests
    {
        private DialogueGraph _graph;
        private DialogueNode _entryNode;
        private DialogueNode _secondNode;

        [SetUp]
        public void SetUp()
        {
            _graph = ScriptableObject.CreateInstance<DialogueGraph>();
            _entryNode = ScriptableObject.CreateInstance<DialogueNode>();
            _secondNode = ScriptableObject.CreateInstance<DialogueNode>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_graph);
            Object.DestroyImmediate(_entryNode);
            Object.DestroyImmediate(_secondNode);
        }

        // Helper: uses reflection to populate the private fields on DialogueNode and DialogueGraph.
        // We could make these internal and expose to tests, but reflection keeps the production
        // API clean.
        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);
            Assert.IsNotNull(field, $"Field '{fieldName}' not found on {target.GetType().Name}.");
            field.SetValue(target, value);
        }

        private void PopulateGraph()
        {
            SetField(_entryNode, "nodeId", "entry");
            SetField(_entryNode, "speakerName", "Angel");
            SetField(_entryNode, "text", "Do not talk to anyone or thing.");
            SetField(_entryNode, "nextNodeId", "second");
            SetField(_secondNode, "nodeId", "second");
            SetField(_secondNode, "speakerName", "Angel");
            SetField(_secondNode, "text", "Now go.");
            SetField(_secondNode, "nextNodeId", string.Empty);

            SetField(_graph, "entryNodeId", "entry");
            SetField(_graph, "nodes", new List<DialogueNode> { _entryNode, _secondNode });
        }

        [Test]
        public void Find_WithExistingId_ReturnsNode()
        {
            PopulateGraph();

            var found = _graph.Find("entry");

            Assert.IsNotNull(found);
            Assert.AreEqual("entry", found.NodeId);
            Assert.AreEqual("Angel", found.SpeakerName);
            Assert.AreEqual("Do not talk to anyone or thing.", found.Text);
        }

        [Test]
        public void Find_WithSecondNodeId_ReturnsSecondNode()
        {
            PopulateGraph();

            var found = _graph.Find("second");

            Assert.IsNotNull(found);
            Assert.AreEqual("second", found.NodeId);
        }

        [Test]
        public void Find_WithMissingId_ReturnsNull()
        {
            PopulateGraph();

            var found = _graph.Find("nonexistent_id");

            Assert.IsNull(found);
        }

        [Test]
        public void Find_WithEmptyId_ReturnsNull()
        {
            PopulateGraph();

            Assert.IsNull(_graph.Find(string.Empty));
        }

        [Test]
        public void Find_WithNullId_ReturnsNull()
        {
            PopulateGraph();

            Assert.IsNull(_graph.Find(null));
        }

        [Test]
        public void Find_WithEmptyGraph_ReturnsNull()
        {
            SetField(_graph, "nodes", new List<DialogueNode>());
            SetField(_graph, "entryNodeId", "anything");

            Assert.IsNull(_graph.Find("anything"));
        }

        [Test]
        public void Find_WithNullNodesList_ReturnsNull()
        {
            SetField(_graph, "nodes", (List<DialogueNode>)null);
            SetField(_graph, "entryNodeId", "entry");

            // Should not throw - just return null.
            Assert.DoesNotThrow(() => _graph.Find("entry"));
            Assert.IsNull(_graph.Find("entry"));
        }

        [Test]
        public void EntryNodeId_ExposesConfiguredValue()
        {
            PopulateGraph();

            Assert.AreEqual("entry", _graph.EntryNodeId);
        }

        [Test]
        public void Find_WithDuplicateIds_ReturnsFirstMatch()
        {
            // If a designer accidentally creates two nodes with the same ID, Find should
            // deterministically return the first one. (Not ideal, but predictable.)
            var duplicateNode = ScriptableObject.CreateInstance<DialogueNode>();
            SetField(duplicateNode, "nodeId", "entry");
            SetField(duplicateNode, "text", "duplicate");

            SetField(_graph, "entryNodeId", "entry");
            SetField(_graph, "nodes", new List<DialogueNode> { _entryNode, duplicateNode });
            SetField(_entryNode, "nodeId", "entry");
            SetField(_entryNode, "text", "original");

            var found = _graph.Find("entry");

            Assert.IsNotNull(found);
            Assert.AreEqual("original", found.Text);

            Object.DestroyImmediate(duplicateNode);
        }
    }
}
