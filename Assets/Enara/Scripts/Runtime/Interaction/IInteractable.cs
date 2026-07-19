using UnityEngine;

namespace Enara.Interaction
{
    /// <summary>
    /// Implement on any GameObject the player can interact with (doors, items, the idol statue,
    /// the icon of Christ, etc). The <see cref="Interaction.Interactor"/> calls these methods.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Short prompt text shown when the player looks at this object ("Press E to talk").</summary>
        string PromptText { get; }

        /// <summary>True if the interaction is currently allowed (e.g. a door isn't locked).</summary>
        bool CanInteract { get; }

        /// <summary>Called when the player presses the interact key while looking at this object.</summary>
        void Interact(GameObject interactor);
    }
}
