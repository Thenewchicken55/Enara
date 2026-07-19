using System.Collections;
using UnityEngine;
using Enara.Core;

namespace Enara.Dialogue
{
    /// <summary>
    /// Plays a <see cref="DialogueGraph"/> node-by-node. Each node shows its speaker/text via
    /// <see cref="UI.Subtitles"/>, optionally plays a VO clip via <see cref="Audio.AudioManager"/>,
    /// and - if the node has choices - waits for the player to pick one before advancing.
    /// </summary>
    public sealed class DialogueRunner : MonoBehaviour
    {
        [SerializeField] private UI.Subtitles subtitles;
        [SerializeField] private Choice.ChoicePresenter choicePresenter;
        [SerializeField] private Audio.AudioManager audioManager;

        private Coroutine _routine;
        private DialogueGraph _graph;
        private DialogueNode _current;

        /// <summary>True while a conversation is actively playing.</summary>
        public bool IsRunning => _routine != null;

        /// <summary>Start playing a graph from its entry node.</summary>
        public void StartDialogue(DialogueGraph graph)
        {
            if (graph == null) { Debug.LogWarning("[DialogueRunner] Null graph."); return; }
            if (IsRunning) StopDialogue();
            _graph = graph;
            if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Dialogue);
            _routine = StartCoroutine(RunGraph(graph));
        }

        /// <summary>Cancel any in-progress conversation.</summary>
        public void StopDialogue()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            _current = null;
            if (subtitles != null) subtitles.Hide();
            if (choicePresenter != null) choicePresenter.Hide();
        }

        private IEnumerator RunGraph(DialogueGraph graph)
        {
            var node = graph.Find(graph.EntryNodeId);
            while (node != null)
            {
                _current = node;
                ShowNode(node);

                if (node.Choices != null && node.Choices.Count > 0)
                {
                    // Wait for the player to pick a choice.
                    string target = null;
                    bool picked = false;
                    if (choicePresenter != null)
                        choicePresenter.Show(node.Choices, choice =>
                        {
                            target = choice.TargetNodeId;
                            EventBus.Publish(new ChoiceMadeEvent(choice.ChoiceId));
                            if (!string.IsNullOrEmpty(choice.FlagToSet))
                                Save.SaveSystem.Instance?.SetFlag(choice.FlagToSet);
                            picked = true;
                        });
                    yield return new WaitUntil(() => picked);
                    node = graph.Find(target);
                }
                else
                {
                    // Auto-advance after a delay.
                    float delay = node.AutoAdvanceDelay > 0f ? node.AutoAdvanceDelay : 3.5f;
                    yield return new WaitForSeconds(delay);
                    node = graph.Find(node.NextNodeId);
                }
            }

            _routine = null;
            _current = null;
            if (subtitles != null) subtitles.Hide();
            if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Exploration);
        }

        private void ShowNode(DialogueNode node)
        {
            if (subtitles != null) subtitles.Show($"{node.SpeakerName}: {node.Text}", node.AutoAdvanceDelay);
            if (audioManager != null && node.VoiceOver != null) audioManager.PlayVoiceOver(node.VoiceOver);
        }
    }
}
