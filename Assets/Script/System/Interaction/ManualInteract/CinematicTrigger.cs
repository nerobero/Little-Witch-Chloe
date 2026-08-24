using UnityEngine;
using UnityEngine.Playables;

public class CinematicTrigger : InteractableBase
{

    public PlayableDirector _director;

    protected override void Interact_Impl()
    {
        _director?.Play();
    }
}
