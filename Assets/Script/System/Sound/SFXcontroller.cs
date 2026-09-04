using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SFXcontroller : MonoBehaviour {

    public GameObject WalkingHerb;
    private void Awake()
    {
        // Auto-assign to this object if not already set in the Inspector
        if (WalkingHerb == null)
            WalkingHerb = gameObject;
    }

    public static object Instance { get; internal set; }

    public void PlayFootstepSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Footstep");
    }

    public void PlayFlightSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/FlyStart");
    }
    public void PlayGlideFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Fly");
    }
    public void PlayAttackSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Attack");
    }
    public void PlayMushroomStartSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Mushroom Mine Grow");

    }
    public void PlayMushroomExplodeSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Mushroom Mine Eplode");

    }
    public void PlayTurnipRunSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Turnip Run", WalkingHerb);



    }
    public void PlayDeathSFX()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Death");


    }
    public void UiHover()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Hover");


    }
    public void UiClick()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Select");


    }
    public void PlayDialouge()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Dialouge");


    }
    public void PlayPoisonCloud()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Poison Cloud");


    }


}
