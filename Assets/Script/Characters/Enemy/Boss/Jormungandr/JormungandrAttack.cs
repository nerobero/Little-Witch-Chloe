using UnityEngine;

public class JormungandrAttack : BossAttack
{
    private JormungandrTail tailRef;
    
    void Start()
    {
        tailRef = GetComponent<JormungandrTail>();
    }

    public void TailAttack(Vector3 targetPosition)
    {
        tailRef.gameObject.SetActive(true);
        tailRef.StartAttack(targetPosition, gameObject);
    }
}
