using System;
using System.Collections.Generic;
using Types;
using UnityEngine;
public class GameManager : MonoSingletonBase<GameManager>
{
    private int collectedFrog = 0;

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedSpell => unlockedSpell;
    public event Action<ECollectable, int> OnObjectivesCollected;
    private Dictionary<ECollectable, int> _commHerbs = new Dictionary<ECollectable, int>();

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SaveManager.Instance.LoadSaveGame();
        foreach(KeyValuePair<ECollectable, int> collection in _commHerbs)
        {
            OnObjectivesCollected?.Invoke(collection.Key, collection.Value);
        }
    }

    #region CollectableCounter
    public void OnFrogCollected()
    {
        if(_commHerbs.ContainsKey(ECollectable.FrogCollectible))
        {
            _commHerbs[ECollectable.FrogCollectible]++;
        }
        else
        {
            _commHerbs.Add(ECollectable.FrogCollectible, 1);
        }
        //collectedFrog++;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
        OnObjectivesCollected?.Invoke(ECollectable.FrogCollectible, _commHerbs[ECollectable.FrogCollectible]);
    }

    public int GetCollectedFrog()
    {
        return collectedFrog;
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
