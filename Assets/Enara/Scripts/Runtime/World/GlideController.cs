using UnityEngine;
using Enara.Core;

namespace Enara.World
{
    /// <summary>
    /// README Path 2 beat: "Idk glide down from the tower trying to get home or something?"
    ///
    /// When active (held down / ability granted), the player descends slowly instead of falling.
    /// Uses the existing CharacterController on the player. Add a small forward thrust so the
    /// player can steer with the mouse.
    ///
    /// Wire the Glide ability's enabled state to a flag set when the player picks up a
    /// hang-glider / robe / etc., or just always allow it for the tower descent section.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class GlideController : MonoBehaviour
    {
        [SerializeField] private Player.PlayerController player;
        [SerializeField] private float glideFallSpeed = -2f;
        [SerializeField] private float forwardThrust = 1.5f;
        [SerializeField] private bool abilityUnlocked = false;
        [SerializeField] private Input.InputReader input;

        private CharacterController _cc;
        private float _verticalVel;

        /// <summary>True if the player is currently gliding.</summary>
        public bool IsGliding { get; private set; }

        /// <summary>Unlock the glide ability at runtime.</summary>
        public void Unlock() { abilityUnlocked = true; }

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (player == null) player = GetComponent<Player.PlayerController>();
            if (input == null) input = FindObjectOfType<Input.InputReader>();
        }

        private void Update()
        {
            if (!abilityUnlocked || _cc.isGrounded) { IsGliding = false; return; }

            // Hold the Interact key (E) to glide, like a parachute deploy.
            bool wantGlide = input != null && (input.InteractPressedThisFrame || Input.GetKey(KeyCode.E));
            IsGliding = wantGlide;

            if (!IsGliding) return;
            // Override gravity: settle on a slow fall + forward push.
            _verticalVel = glideFallSpeed;
            _cc.Move(transform.forward * forwardThrust * Time.deltaTime);
            _cc.Move(Vector3.up * _verticalVel * Time.deltaTime);
        }
    }
}
