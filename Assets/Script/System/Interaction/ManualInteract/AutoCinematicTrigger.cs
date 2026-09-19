using UnityEngine;
using UnityEngine.Playables;

public class AutoCinematicTrigger : EventTriggerBase, IResetable
{
    public PlayableDirector _director;
    private bool _hasInteracted = false;

    public void ResetState() => _hasInteracted = false;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!_hasInteracted)
        {
            _hasInteracted = true;
            _director?.Play();
        }
    }
}
