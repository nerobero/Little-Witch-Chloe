using UnityEngine;

/// <summary>
/// This is a global enum type namespace
/// </summary>
namespace Types
{
    /// <summary>
    /// Defines a type of enemy
    /// </summary>
    public enum EElementType // Have to rename EElementType
    {
        None = 0,
        Fire,
        Plant,
        Poison,
        Darkness,
        Ice,
        Light,
        Electricity,
        Water,
    }

    /// <summary>
    /// Defines a state of enemy
    /// </summary>
    public enum EMonsterState
    {
        Idle = 0,   // Idle state(do nothing)
        Patrol,     // Patrol
        Chase,      // Chase the target
        Attack,     // Attack the target
        Dead,      // Dead state
        Seen, // Some monsters have a separate interaction with the player when 'seen'
    }


    /// <summary>
    /// Defines a type of pool object that can be spawned
    /// and be managed by the PoolObjectManager
    /// </summary>
    public enum ESpawnType
    {
        // PC normal projectiles (excluding the charged attacks).
        // NPCs can also spawn these projectiles.
        FireBall = 0,
        WaterBall,
        ElectricBall,
        PoisonBall,
        LightBall,
        // NPC projectiles
        FireSlash,
        FirePillar,
        PlantBall,
        PoisonPool,
        PoisonSplash,
        DarknessBall,
        DarknessCloud,
        IceShard,
        IceBall,
        IceGroundShards,
        ScrollItem,
        HPItem,
        StaminaItem,
    }

    /// <summary>
    /// Defines a type of buff that can be granted
    /// and be managed by the stat
    /// </summary>
    public enum EBuffType
    {
        None = 0,
        Empowere,
        
    }

    /// <summary>
    /// Defines an spell that can be granted by scroll
    /// </summary>
    public enum EAbilityType
    {
        None = 0,
        Flying,
        Blink,
        
    }
}