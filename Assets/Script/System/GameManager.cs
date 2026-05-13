using System.Collections.Generic;
using UnityEngine;
using Types;
public class GameManager : MonoBehaviour
{
    // make Game Manager to singleton
    public static GameManager Instance {get; private set;}

    private int collectedFrog;

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            // maintain this instance even if the scene changed.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
