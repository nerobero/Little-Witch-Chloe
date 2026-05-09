using UnityEngine;

public class MushroomMineAnimController : EnemyAnimController
{
    private static readonly int IsGrown = Animator.StringToHash("IsGrown");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetToStartDead()
    {
        _animator.SetBool(IsDead, true);
    }

     public void SetToRestart()
    {
        _animator.SetBool(IsDead, false);
        _animator.SetBool(IsGrown, false);
    }

    public void SetToStartGrowing()
    {
        _animator.SetBool(IsGrown, true);
    }

    public void SetToStartShrinking()
    {
        _animator.SetBool(IsGrown, false);
    }
}
