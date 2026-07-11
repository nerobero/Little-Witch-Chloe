using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that manages the player save data-based 
/// scene initialization.
/// </summary>
public class SceneBase : MonoBehaviour
{
    private readonly List<IResetable> resetables = new List<IResetable>();
    #if UNITY_EDITOR
    [SerializeField] private List<MonoBehaviour> check = new List<MonoBehaviour>();
    #endif

    public void Start()
    {
        LevelManager.Instance.Register(this);
        
        Initialize();
    }

    public void Initialize()
    {
        Debug.Log("yeap");
        var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var behaviour in behaviours)
        {
            if (behaviour is IResetable resetable)
            {
                resetables.Add(resetable);
                #if UNITY_EDITOR
                check.Add(behaviour);
                #endif
            }
        }
    }

    public void ResetScene()
    {
        foreach(var resetable in resetables)
        {
            Debug.Log("SceneBase: " + resetable + " Reset!");
            resetable.ResetState();
        }
    }

    // public void ApplyAllGameData()
    // {
        
    // }
}
