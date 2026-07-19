using UnityEngine;
using Enara.Core;

namespace Enara.Interaction
{
    /// <summary>
    /// Casts a ray from the player camera and finds the <see cref="IInteractable"/> under the
    /// crosshair. Drives <see cref="InteractionPromptUI"/> to show/hide the prompt and triggers
    /// the interaction when the player presses the interact key.
    /// </summary>
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float maxReach = 2.5f;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private InteractionPromptUI promptUI;

        private Input.InputReader _input;
        private IInteractable _current;

        private void Reset()
        {
            if (cam == null) cam = GetComponentInChildren<Camera>(true);
            if (promptUI == null) promptUI = FindObjectOfType<InteractionPromptUI>(true);
        }

        private void Awake()
        {
            if (cam == null) cam = GetComponentInChildren<Camera>(true);
            if (promptUI == null) promptUI = FindObjectOfType<InteractionPromptUI>(true);
            _input = Input.InputReader.Instance;
        }

        private void Update()
        {
            UpdateCurrentTarget();

            if (_current != null && _current.CanInteract && _input != null && _input.InteractPressedThisFrame)
                _current.Interact(gameObject);
        }

        private void UpdateCurrentTarget()
        {
            IInteractable next = null;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, maxReach, interactionMask))
            {
                next = hit.collider.GetComponentInParent<IInteractable>();
                if (next != null && !next.CanInteract) next = null;
            }

            if (!ReferenceEquals(next, _current))
            {
                _current = next;
                if (promptUI != null)
                {
                    if (_current != null) promptUI.Show(_current.PromptText);
                    else promptUI.Hide();
                }
            }
        }
    }
}
