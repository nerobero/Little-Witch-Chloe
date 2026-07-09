using UnityEngine;

public class WaitForSecondsTracked : CustomYieldInstruction
{
    private float targetTime;

    // Expose the remaining time safely
    public float TimeRemaining => Mathf.Max(0, targetTime - Time.time);

    // This overrides the condition that tells Unity when to stop waiting
    public override bool keepWaiting => Time.time < targetTime;

    public WaitForSecondsTracked(float duration)
    {
        targetTime = Time.time + duration;
    }

    public void Reset(float duration)
    {
        targetTime = Time.time + duration;
    }
}
