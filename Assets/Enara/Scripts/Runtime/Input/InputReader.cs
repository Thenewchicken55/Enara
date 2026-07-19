using UnityEngine;
using UnityEngine.InputSystem;

namespace Enara.Input
{
    /// <summary>
    /// Thin wrapper around the PlayerControls InputActionAsset (Resources/PlayerControls).
    /// Exposes simple properties like MoveInput / LookDelta / InteractPressedThisFrame so
    /// gameplay code never touches the InputSystem directly.
    ///
    /// Wiring:
    ///   1. Place this component on a GameObject in the Boot scene (or each scene with a player).
    ///   2. Drag the PlayerControls asset into the actionsAsset slot (auto-loaded from Resources).
    /// </summary>
    public sealed class InputReader : MonoBehaviour
    {
        /// <summary>Per-process singleton. Set in Awake. Null if no InputReader exists.</summary>
        public static InputReader Instance { get; private set; }

        [SerializeField] private InputActionAsset actionsAsset;
        [SerializeField] private bool enableOnStart = true;
        [SerializeField] private string playerMapName = "Player";

        private InputAction _move;
        private InputAction _look;
        private InputAction _interact;
        private InputAction _advance;
        private InputAction _pause;

        /// <summary>Current move vector (x = strafe, y = forward). 0 if no input.</summary>
        public Vector2 MoveInput => _move != null ? _move.ReadValue<Vector2>() : Vector2.zero;

        /// <summary>Mouse/stick delta for the current frame. Multiply by sensitivity.</summary>
        public Vector2 LookDelta => _look != null ? _look.ReadValue<Vector2>() : Vector2.zero;

        /// <summary>True on the frame the player pressed the Interact key (E / gamepad A).</summary>
        public bool InteractPressedThisFrame => _interact != null && _interact.WasPressedThisFrame();

        /// <summary>True on the frame the player pressed Advance (Space / mouse / gamepad A).</summary>
        public bool AdvancePressedThisFrame => _advance != null && _advance.WasPressedThisFrame();

        /// <summary>True on the frame the player pressed Pause (Escape / Start).</summary>
        public bool PausePressedThisFrame => _pause != null && _pause.WasPressedThisFrame();

        private void Reset()
        {
            if (actionsAsset == null) actionsAsset = Resources.Load<InputActionAsset>("PlayerControls");
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (actionsAsset == null) actionsAsset = Resources.Load<InputActionAsset>("PlayerControls");
            if (actionsAsset == null)
            {
                Debug.LogError("[InputReader] PlayerControls.inputactions missing from Resources.");
                return;
            }
            var map = actionsAsset.FindActionMap(playerMapName);
            if (map == null) { Debug.LogError($"[InputReader] Action map '{playerMapName}' not found."); return; }
            _move = map.FindAction("Move", throwIfNotFound: false);
            _look = map.FindAction("Look", throwIfNotFound: false);
            _interact = map.FindAction("Interact", throwIfNotFound: false);
            _advance = map.FindAction("Advance", throwIfNotFound: false);
            _pause = map.FindAction("Pause", throwIfNotFound: false);
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void OnEnable()
        {
            if (enableOnStart && actionsAsset != null) actionsAsset.Enable();
        }

        private void OnDisable()
        {
            if (actionsAsset != null) actionsAsset.Disable();
        }
    }
}
