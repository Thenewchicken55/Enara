using UnityEngine;
using Enara.Core;

namespace Enara.Player
{
    /// <summary>
    /// First-person player controller. Drives movement with a CharacterController (so we get
    /// slope handling and collisions for free), reads input from <see cref="Input.InputReader"/>,
    /// and exposes a <see cref="Limping"/> flag so the README beat - "injured, limps to find help" -
    /// can drop the player's speed.
    ///
    /// Locks itself when the global <see cref="GameState"/> isn't Exploration or Qte.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private FirstPersonLook look;
        [SerializeField] private float gravity = -19.6f;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundMask = ~0;

        private CharacterController _cc;
        private Input.InputReader _input;
        private Core.GameSettings _settings;
        private GameStateMachine _state;
        private Vector3 _velocity;

        /// <summary>True while the player is limping (injured). Drops movement speed.</summary>
        public bool Limping { get; set; } = true;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = FindObjectOfType<Input.InputReader>();
            _settings = Core.GameSettings.LoadOrDefault();
            _state = Core.GameManager.Instance != null ? Core.GameManager.Instance.State : null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            if (_state != null) _state.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (_state != null) _state.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            bool movementAllowed = next == GameState.Exploration || next == GameState.Qte;
            enabled = movementAllowed;
        }

        private void Update()
        {
            HandleLook();
            HandleMove();
        }

        private void HandleLook()
        {
            if (_input == null || look == null) return;
            var sensitivity = _settings != null ? _settings.LookSensitivity : 0.05f;
            look.ApplyLook(_input.LookDelta * sensitivity);
        }

        private void HandleMove()
        {
            if (_input == null) return;
            var inputDir = _input.MoveInput;          // Vector2: x = strafe, y = forward
            var move = transform.right * inputDir.x + transform.forward * inputDir.y;
            if (move.sqrMagnitude > 1f) move.Normalize();

            float speed = Limping
                ? (_settings != null ? _settings.LimpSpeed : 0.9f)
                : (_settings != null ? _settings.WalkSpeed : 1.6f);

            _cc.Move(move * speed * Time.deltaTime);

            // Gravity. Reset downward velocity when grounded so we don't accumulate forever.
            _velocity.y += gravity * Time.deltaTime;
            if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _cc.Move(_velocity * Time.deltaTime);
        }
    }
}
