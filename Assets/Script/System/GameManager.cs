using System;
using System.Collections.Generic;
using System.Linq;
using Types;
using UnityEngine;
public class GameManager : MonoSingletonBase<GameManager>
{

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();
    private ELevelType _currentLevel;
    public ELevelType CurrentLevel => _currentLevel;

    // Activated spells by scroll.
    private HashSet<EAbilityType> _unlockedAbilities = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedAbilities => _unlockedAbilities;
    public List<EAbilityType> GetUnlockedAbilitiesList => new List<EAbilityType>(_unlockedAbilities);

    // Objectives related fields
    public event Action<ECollectable, int> OnObjectivesCollected;
    private HashSet<string> _defeatedBosses = new HashSet<string>();

    private Dictionary<ECollectable, int> _objectives = new Dictionary<ECollectable, int>();




    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnUnlockLevel += OnUnlockLevel;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnUnlockLevel -= OnUnlockLevel;
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
        if (_objectives.ContainsKey(ECollectable.FrogCollectible))
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

    public void RestoreObjective(ECollectable type, int count)
    {
        if (_objectives.ContainsKey(type))
        {
            _objectives[type] = count;
            OnObjectivesCollected?.Invoke(type, count);
        }
    }
    #endregion

    #region LevelLoad

    public bool IsLevelUnlocked(ELevelType level)
    {
        return unlockedLevels.Contains(level);
    }

    public void OnUnlockLevel(ELevelType type)
    {
        if (!unlockedLevels.Contains(type))
            unlockedLevels.Add(type);
    }

    public void SetCurrentLevel(ELevelType type) => _currentLevel = type;

    #endregion

    #region ScrollCollection

    /// <summary>
    /// Manage the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="scrollType">the ability to unlock</param>
    /// <returns>Does ability unlocked succeed</returns>
    public bool OnScrollCollected(EAbilityType scrollType)
    {
        return _unlockedAbilities.Add(scrollType);
    }

    /// <summary>
    /// Check the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="spell">the ability to find<</param>
    /// <returns>Is ability unlocked</returns>
    public bool IsSpellUnlocked(EAbilityType spell)
    {
        return _unlockedAbilities.Contains(spell);
    }

    #endregion
}
