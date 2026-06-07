using System;
using System.Collections.Generic;
using Types;
using UnityEngine;
public class GameManager : MonoSingletonBase<GameManager>
{

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedSpell => unlockedSpell;

    public event Action<ECollectable, int> OnObjectivesCollected;

    private Dictionary<ECollectable, int> _objectives = new Dictionary<ECollectable, int>();

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SaveManager.Instance.LoadSaveGame();
        foreach (KeyValuePair<ECollectable, int> collection in _objectives)
        {
            OnObjectivesCollected?.Invoke(collection.Key, collection.Value);
        }
    }

    #region CollectableCounter
    public void OnFrogCollected()
    {
        if(_objectives.ContainsKey(ECollectable.FrogCollectible))
        {
            _objectives[ECollectable.FrogCollectible]++;
        }
        else
        {
            _objectives.Add(ECollectable.FrogCollectible, 1);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
        OnObjectivesCollected?.Invoke(ECollectable.FrogCollectible, _objectives[ECollectable.FrogCollectible]);
    }

    public int GetCollectedFrog()
    {
        if (!_objectives.ContainsKey(ECollectable.FrogCollectible))
            return 0;
        return _objectives[ECollectable.FrogCollectible];
    }

    public bool OnAntiFogMossCollected()
    {
        if (_objectives.ContainsKey(ECollectable.AntiFogMossPatch))
            _objectives[ECollectable.AntiFogMossPatch]++;
        else _objectives.Add(ECollectable.AntiFogMossPatch, 1);
        
        return true;
    }

    public bool OnCommHerbCollected(ECollectable type)
    {
        if (type == ECollectable.FrogCollectible
            || type == ECollectable.AntiFogMossPatch)
            return false;

        if (_objectives.ContainsKey(type))
        {
            _objectives[type]++;
        }
        else
        {
            _objectives.Add(type, 1);
        }

        return true;
    }


    #endregion

    #region LevelLoad

    public bool IsLevelUnlocked(ELevelType level)
    {
        return unlockedLevels.Contains(level);
    }

    // @TODO: implement a function that actually processes the unlocking (i.e., adding a new level) to the hash set:

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
