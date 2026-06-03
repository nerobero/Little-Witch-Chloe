using System.Collections.Generic;
using System;
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
        Dead,       // Dead state
        Seen,       // Some monsters have a separate interaction with the player when 'seen'
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
        WaterBallNPC,
        PlantBall,
        PoisonPool,
        PoisonSplash,
        PoisonBallNPC,
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

    public enum ELevelType
    {
        Intro = 0,
        MainGame,
        BogLevel,

    }

    /// <summary>
    /// Defines types of collectables that needs management by Game Manager
    /// </summary>
    public enum ECollectable
    {
        FrogCollectible = 0,
        // ========== Herb types for commissions ===========
        CommDigestHerb,
        CommColdHerb,
        CommFeverHerb,
        // ===========  Moss Patches for Bog Level ===========
        AntiFogMossPatch,
    }
}

/// <summary>
/// This is a global  type namespace
/// </summary>
namespace Data
{
    // Save data : unlocked spell, inventory?
    [System.Serializable]
    public class SavePlayerData
    {
        public HashSet<Types.EAbilityType> unlockedAbility; // Blink, Flying
        public HashSet<Types.ESpawnType> spellList; // Projectiles
        public Transform savedTransform;
        public DateTime currentTime;
    }

    // Setting data : Master volume, graphics, input key?
    [System.Serializable]
    public class SettingData
    {
        // sound setting
        // master volume
        public float masterVolume;
        // bgm volume
        public float bgmVolume;
        // effect volume
        public float sfxVolume;
        // public int resolutionWidth;
        // public int resolutionHeight;
        // public bool isFullScreen;

        // Language setting
        public SystemLanguage language;
    }

}