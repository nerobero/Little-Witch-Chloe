using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SFXcontroller : MonoBehaviour {
    
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/FlyStart");
    }
    public void PlayAttackSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Attack");
    }
    public void PlayMushroomStartSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/event:/SFX/Mushroom Mine Grow");

    }
    public void PlayMushroomExplodeSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Mushroom Mine Eplode");

    }

}
