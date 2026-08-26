using UnityEngine;
using UnityEngine.Playables;

public class CinematicTrigger : InteractableBase
{

    protected bool _hasInteracted = false;

    public PlayableDirector _director;

    protected override void Interact_Impl()
    {
        if (!_hasInteracted)
        {
            _hasInteracted = true;
            _director?.Play();
        }
        
    }

    public override void ResetState()
    {
        base.ResetState();
        _hasInteracted = false;
    }
}
