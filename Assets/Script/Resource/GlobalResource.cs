using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Types;
using Unity.VisualScripting;

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

    public enum EBossState
    {
        None = 0,
        Idle,       // Normal State(Not in the boss field)
        Attack,     // Attack
        Rampage,      // HP under 50%
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
        Collections,
        None, // the end and nothing
    }

    /// <summary>
    /// Defines a type of buff or debuff effect that can be granted
    /// and be managed by the stat
    /// </summary>
    public enum EStatusEffectType
    {
        None = 0,
        // Buff
        AttackUp,
        DefenseUp,
        MoveSpeedUp,
        AttackSpeedUp,
        Shield,
        AntiPoisonFog,

        // Debuff
        AttackDown,
        DefenseDown,
        Slow,
        AttackSpeedDown,
        //Bleeding,

        // Crowd Control
        Stun,
        Freeze,
        Root,
        Silence,
        Knockback,
        Fear,
        Blind,

    }

    public enum EStackRule
    {
        RefreshDuration, // if the same effect, refresh the duration
        Stack,           // add the stacks
        Replace,         // remove the older one, and apply the new one
        Ignore           // ignore the newer one
    }

    /// <summary>
    /// Defines a category of buff / debuff / Crowd Control effect that can be granted
    /// </summary>
    public enum EStatusEffectCategory
    {
        Buff,
        Debuff,
        CrowdControl,
    }

    [Flags]
    public enum ECrowdControlType
    {
        None            = 0,
        Rooted          = 1 << 0,
        Stunned         = 1 << 1,
        Silenced        = 1 << 2,
        Blinded         = 1 << 3,
        Knockedback     = 1 << 4,
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
        Overworld,
        BogLevel,

        // Last for level amount
        Count,
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
/// This is a global struct or class data namespace
/// </summary>
namespace Data
{
    // Save data : unlocked spell, inventory?
    [System.Serializable]
    public class SavePlayerData
    {
        // ============= LEVEL PLACEMENT related data ==================
        public ELevelType currentLevel; // the level the player was last playing in
        public List<Types.ELevelType> unlockedLevels;

        // Breaking the Transform into two parts; Transform is NOT Json-serializable:
        public Vector3 savedPosition;
        public Quaternion savedRotation;
        public Vector3 savedScale;
        public bool isInBackground; // added this field to determine whether to be placed in fg or bg

        // ============== PLAYER related data ==================
        public List<Types.EAbilityType> unlockedAbility; // Blink, Flying
        public List<Types.ESpawnType> spellList; // unlocked Projectile spells
        public List<SavedObjectiveData> objectives;
        public List<string> defeatedBosses;
        public float currentHP;
        public float currentMaxHP;
        public float currentStamina;

        // ============== LOGISTICS =======================
        public DateTime currentTime;
    }

    [System.Serializable]
    public class SavedObjectiveData
    {
        public Types.ECollectable collectableType;
        public int collectedCount = 0;

        public SavedObjectiveData() {}

        public SavedObjectiveData(Types.ECollectable type, int amount)
        {
            collectableType = type;
            collectedCount = amount;
        }
    }
    public class ObjectiveData
    {
        public Types.ECollectable collectableType;
        public int collectedCount = 0;

        public ObjectiveData() {}

        public ObjectiveData(Types.ECollectable type, int amount)
        {
            collectableType = type;
            collectedCount = amount;
        }
    }

    [System.Serializable]
    public class CollectableData : IBinaryRecord
    {
        public ELevelType levelType;
        public ECollectable collectableType;
        public int collectedCount = 0;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((uint)levelType);
            writer.Write((uint)collectableType);
            writer.Write(collectedCount);
        }

        public static CollectableData Deserialize(BinaryReader reader) => new CollectableData
        {
            levelType       = (ELevelType)reader.ReadUInt32(),
            collectableType = (ECollectable)reader.ReadUInt32(),
            collectedCount  = reader.ReadInt32(),
        };
    }

    public struct DialogueRow : IBinaryRecord
    {
        public uint currentridx;
        public string speakerName;
        public string dialogueText;
        public uint nextridx;
        public bool hasDialogueEnded;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(currentridx);
            writer.Write(speakerName);
            writer.Write(dialogueText);
            writer.Write(nextridx);
            writer.Write(hasDialogueEnded);
        }

        public static DialogueRow Deserialize(BinaryReader reader) => new DialogueRow
        {
            currentridx      = reader.ReadUInt32(),
            speakerName      = reader.ReadString(),
            dialogueText     = reader.ReadString(),
            nextridx         = reader.ReadUInt32(),
            hasDialogueEnded = reader.ReadBoolean(),
        };
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

    public struct BuffData
    {
        public StatManager owner;
        public float lifeTime;
        public Action onEnd;

        public BuffData(StatManager owner, float lifeTime, Action onEnd)
        {
            this.owner = owner;
            this.lifeTime = lifeTime;
            this.onEnd = onEnd;
        }
    }
}
    