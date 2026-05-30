using System.Collections.Generic;
using Types;
public class GameManager : MonoSingletonBase<GameManager>
{
    // make Game Manager to singleton
    // public static GameManager Instance {get; private set;}

    private int collectedFrog;

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedSpell => unlockedSpell;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveManager.Instance.LoadSaveGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFrogCollected()
    {
        collectedFrog++;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
    }

    /// <summary>
    /// Manage the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="scrollType">the ability to unlock</param>
    /// <returns>Does ability unlocked succeed</returns>
    public bool OnScrollCollected(EAbilityType scrollType)
    {
        return unlockedSpell.Add(scrollType);
    }

    /// <summary>
    /// Check the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="spell">the ability to find<</param>
    /// <returns>Is ability unlocked</returns>
    public bool IsSpellUnlocked(EAbilityType spell)
    {
        return unlockedSpell.Contains(spell);
    }
}
