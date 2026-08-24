using UnityEngine;


public class Dialogueable : InteractableBase
{
    protected override void Interact_Impl()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanInteract()
    {
        return base.CanInteract();
    }
}
