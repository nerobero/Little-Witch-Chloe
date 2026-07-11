using UnityEngine;
using Types;

public interface IDamageable
{
    public bool TakeDamageHelper(GameObject instigator, float damageAmount, EElementType damageElement,
                                            bool isDOT = false, float duration = 0f, float interval = 0f);
}
