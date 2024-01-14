using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnDamageEvent: UnityEvent<int> { }
public class HPSystem : MonoBehaviour,IDamageable
{
    [SerializeField]
    private int maxHp = 10;
    [SerializeField]
    private UnityEvent OnHpLimitReachedEvent = new UnityEvent();
    [SerializeField]
    private OnDamageEvent OnDamageDoneEvent = new OnDamageEvent();
    private int currentHP = 0;
    private bool hpLimitReached = false;
    private void OnEnable()
    {
        Initialize();
    }
    private void Initialize()
    {
        currentHP = maxHp;
        hpLimitReached = false;
    }
    public void OnHPLimitReached()
    {
        OnHpLimitReachedEvent?.Invoke();
    }

    public void ReceiveDamage(int damageValue)
    {
        if (hpLimitReached)
        {
            return;
        }
        currentHP -= damageValue;
        if (currentHP <= 0)
        {
            currentHP = 0;
            OnHPLimitReached();
        }
        OnDamageDoneEvent?.Invoke(currentHP);
    }


}
