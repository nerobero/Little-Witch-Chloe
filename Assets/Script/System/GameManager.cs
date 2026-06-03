using System.Collections.Generic;
using Types;
public class GameManager : MonoSingletonBase<GameManager>
{

    private int collectedFrog;

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedSpell => unlockedSpell;

    private Dictionary<ECollectable, int> _commHerbs = new Dictionary<ECollectable, int>();

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

    #region CollectableCounter
    public void OnFrogCollected()
    {
        collectedFrog++;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
    }

    public bool OnCommHerbCollected(ECollectable type)
    {
        if (type == ECollectable.FrogCollectible
            || type == ECollectable.AntiFogMossPatch)
            return false;
        return true;
    }


    #endregion

    #region LevelLoad

    public bool IsLevelUnlocked(ELevelType level)
    {
        return unlockedLevels.Contains(level);
    }

    #endregion

    #region ScrollCollection

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

    #endregion
}
