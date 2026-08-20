using UnityEngine;

public class BurlAnimController : EnemyAnimController
{
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
    }
}
