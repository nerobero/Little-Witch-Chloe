using UnityEngine;
using Types;

public class BenchMark : MonoBehaviour, IResetable
{
    // current level type to know where this benchmark is placed
    ELevelType levelType;
    bool isPassed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelManager.Instance.RegisterInstance(this);
        levelType = GameManager.Instance.CurrentLevel;
        LevelManager.Instance.RegisterBenchMark(levelType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isPassed) return;

        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);
        
        if(otherLayer.Contains("Player"))
        {
            isPassed = true;
            int totalBenchmarksForLevel = LevelManager.Instance.GetTotalBenchmarksForLevel(levelType);
            float normalizedIncrement = 1.0f / totalBenchmarksForLevel;
            LovePotionManager.Instance.OnLevelBenchmarkPassed(normalizedIncrement);
        }
    }

    public void ResetState()
    {
        isPassed = false;
    }
}
