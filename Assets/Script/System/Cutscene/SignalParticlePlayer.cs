using UnityEngine;

/// <summary>
/// Plays its own ParticleSystem via a Signal, bypassing Timeline entirely - a Control Track
/// clip would tie the particle system's simulation to the PlayableDirector's play/pause state,
/// which is exactly what we don't want while the director is paused for the crafting QTE.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class SignalParticlePlayer : MonoBehaviour
{
    private ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    public void PlayParticles()
    {
        _particles.Play();
    }

    public void StopParticles()
    {
        _particles.Stop();
    }
}
