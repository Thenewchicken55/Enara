using UnityEngine;

namespace Enara.World
{
    /// <summary>
    /// Plays footstep audio based on the player's movement state. The README's limp beat calls
    /// for a heavier, more labored footstep; the post-healing walk is lighter. Pair this with
    /// the <see cref="Player.PlayerController"/> on the player GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FootstepController : MonoBehaviour
    {
        [SerializeField] private AudioClip[] limpSteps;
        [SerializeField] private AudioClip[] walkSteps;
        [SerializeField] private float limpStepInterval = 0.9f;
        [SerializeField] private float walkStepInterval = 0.5f;
        [SerializeField] private Player.PlayerController player;
        [SerializeField] private Audio.AudioManager audioManager;

        private CharacterController _cc;
        private float _stepTimer;

        private void Reset()
        {
            if (player == null) player = GetComponent<Player.PlayerController>();
            if (audioManager == null) audioManager = FindObjectOfType<Audio.AudioManager>();
        }

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (player == null) player = GetComponent<Player.PlayerController>();
            if (audioManager == null) audioManager = FindObjectOfType<Audio.AudioManager>();
        }

        private void Update()
        {
            if (!_cc.isGrounded) { _stepTimer = 0f; return; }
            float horizontalSpeed = new Vector2(_cc.velocity.x, _cc.velocity.z).magnitude;
            if (horizontalSpeed < 0.1f) { _stepTimer = 0f; return; }

            _stepTimer -= Time.deltaTime;
            if (_stepTimer > 0f) return;

            bool limping = player != null && player.Limping;
            var pool = limping ? limpSteps : walkSteps;
            float interval = limping ? limpStepInterval : walkStepInterval;
            if (pool != null && pool.Length > 0 && audioManager != null)
            {
                var clip = pool[Random.Range(0, pool.Length)];
                audioManager.PlaySfx(clip);
            }
            _stepTimer = interval;
        }
    }
}
