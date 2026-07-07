using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JormungandrTailAnim : MonoBehaviour
{

    protected Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetToIdle()
    {
        gameObject.SetActive(false);
    }
}
