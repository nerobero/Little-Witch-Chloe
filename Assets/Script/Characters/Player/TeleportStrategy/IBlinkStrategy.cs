using UnityEngine;

/// <summary>
/// Strategy interface for calculating the next position vector 
/// when Chloe teleports
/// </summary>
public interface IBlinkStrategy
{
    public void SetUpStrategyParams(LayerMask fg, LayerMask bg, float angle);
    public Vector2 ProcessTeleport(float alpha, bool isBackground, bool isFacingRight,
                                   Transform characOrigin, Transform platformOrigin);

}
