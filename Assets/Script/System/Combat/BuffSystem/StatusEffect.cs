using UnityEngine;
using Types;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "ScriptableObject/StatusEffect")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public MonoBehaviour owner; // Monobehaviour?
        
        [SerializeField] protected EStatusEffectType type;
        public EStatusEffectType Type => type;
        
        [SerializeField] protected EStatusEffectCategory category;
        public EStatusEffectCategory Category => category;

        [SerializeField] protected ECrowdControlType ccType;
        public ECrowdControlType CCType => ccType;

        [SerializeField] protected Sprite icon;
        public Sprite Icon => icon;
        [SerializeField] protected string effectName;
        public string EffectName => effectName;

        /// <summary>
        /// The amount of buff/debuff(status) effect. (like N% slowed).
        /// </summary>
        [SerializeField] protected float magnitude;
        public float Magnitude => magnitude;

        [SerializeField] protected float duration;
        public float Duration => duration;

        //public float remainingTime { get; protected set;}

        public StatusEffect(MonoBehaviour owner, EStatusEffectType type, EStatusEffectCategory category,
                            ECrowdControlType ccType, Sprite icon, string effectName, float magnitude, float duration)
        {
            this.owner = owner;
            this.type = type;
            this.category = category;
            this.ccType = ccType;
            this.icon = icon;
            this.effectName = effectName;
            this.magnitude = magnitude;
            this.duration = duration;
        }

        public virtual void Apply(StatManager target, GameObject instigator)
        {
            Debug.Log("Applied");
            target.Buff(this, instigator);
        }

        // public virtual void Tick(StatManager target, float deltaTime)
        // {
        //     remainingTime -= deltaTime;
        // }

        // public virtual void Expire()
        // {
        //     remainingTime = 0.0f; 
        // }

        public virtual void Remove(StatManager target)
        {
            target.RemoveBuff(this);
        }
        
    }
