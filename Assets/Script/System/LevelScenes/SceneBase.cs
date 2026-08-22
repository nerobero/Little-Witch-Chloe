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

    [SerializeField] private string FMODEvent = "";

    public void Start()
    {
        LevelManager.Instance.Register(this);

        // @SHIORI: put the play fmod event logic here:
        
        //Initialize();
    }

    // public void Initialize()
    // {
    //     Debug.Log("yeap");
    //     var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

    //     foreach (var behaviour in behaviours)
    //     {
    //         if (behaviour is IResetable resetable)
    //         {
    //             resetables.Add(resetable);
    //             #if UNITY_EDITOR
    //             check.Add(behaviour);
    //             #endif
    //         }
    //     }
    // }

    public void ResetScene()
    {
        for(int i = resetables.Count - 1; i >= 0; --i)
        {
            Debug.Log("SceneBase: " + resetables[i] + " Reset!");
            resetables[i].ResetState();
        }
    }

    public void Register(MonoBehaviour behaviour)
    {
        if (behaviour is IResetable resetable)
        {
            resetables.Add(resetable);
            #if UNITY_EDITOR
            check.Add(behaviour);
            #endif
        }
    }

    // public void ApplyAllGameData()
    // {
        
    // }
}
