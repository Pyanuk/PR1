using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int Health;

    private int MaxHealth = 100;

    private int Stamina = 100;

    public bool IsStaminaRestoring = false;

    private void Start()
    {
         Health = MaxHealth;

    }

    public void SpendStamina()
    {
        Stamina -= 1;
    }

    private IEnumerator StaminaRestore()
    {
        IsStaminaRestoring = true;
        yield return new WaitForSeconds(3);
        Stamina = 100;
        IsStaminaRestoring = false;
    }

    private void StaminaCheck()
    {
        Debug.Log("Stamina" + Stamina);
        if(Stamina <= 0) StartCoroutine(StaminaRestore());

    }

    private void FixedUpdate()
    {
        StaminaCheck();
    }

    public void Healing(int HealthPointCount)
    {
        if (Health + HealthPointCount > MaxHealth)
        {
            Health = MaxHealth;
        }
        else Health += Health += HealthPointCount;

        Debug.Log("HP:" + Health);
    }

    public void Damager(int DamagePointCount)
    {
        Health -= DamagePointCount;

        if(Health < 0)
        {
            Health = 0;
        }

        Debug.Log("HP:" + Health);

        if (Health <= 0)
        {
            Debug.Log("Player Died");
        }
    }
}
