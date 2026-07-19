using UnityEngine;
using UnityEngine.Playables;

namespace Enara.Cutscene
{
    /// <summary>
    /// Receives Signal Emitter events from a Timeline and forwards them as UnityEvents or
    /// direct method calls. Place this component on the same GameObject as the
    /// <see cref="PlayableDirector"/> and reference it in each Signal Receiver on the timeline.
    ///
    /// Example use: at the crash frame in the intro Timeline, fire a "CrashImpact" signal
    /// that calls <see cref="CameraShake.Shake(float)"/> and triggers a crash SFX.
    /// </summary>
    public sealed class CutsceneSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private SignalAsset[] handledSignals;
        [SerializeField] private UnityEngine.Events.UnityEvent[] onSignal;

        /// <summary>Called by Unity's Timeline system when a SignalEmitter fires.</summary>
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (!(notification is SignalEmitter emitter) || emitter.asset == null) return;
            for (int i = 0; i < handledSignals.Length; i++)
            {
                if (i < onSignal.Length && handledSignals[i] == emitter.asset)
                    onSignal[i]?.Invoke();
            }
        }
    }
}
