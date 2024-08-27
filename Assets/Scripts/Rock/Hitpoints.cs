using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitpoints : MonoBehaviour
{
    [SerializeField]
    private float hp;
    private float totalHp;

    [SerializeField]
    private UnityEvent onDeath;

    public UnityEvent<float> event_UpdateHealth;

    public UnityEvent<float> onDamageReceived;

    void Start()
    {
        totalHp = hp;
    }

    void Update()
    {
        //Damage(Time.deltaTime * 50f);
    }

    public void Damage(float d = 1f)
    {
        hp = Mathf.Clamp(hp - d, 0, totalHp);

        CheckDeath();

        InvokeUpdateHealthEvent();

        onDamageReceived?.Invoke(d);
    }

    public void Heal(float e = 1f)
    {
        hp = Mathf.Clamp(hp + e, 0, totalHp);

        InvokeUpdateHealthEvent();
    }

    void CheckDeath()
    {
        if (hp <= 0)
        {
            onDeath.Invoke();
        }
    }

    public void Test()
    {
        Debug.Log("Dead!");
    }

    private void InvokeUpdateHealthEvent()
    {
        event_UpdateHealth?.Invoke(hp / totalHp);
    }

    //public void GameOver()
    //{
    //    GameManager.GM?.GameOver();
    //}

}
