using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int WeaponId; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
 
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
             
                playerController.UnlockWeapon(WeaponId);

                Debug.Log("Оружие разблокировано: " + WeaponId);
                Destroy(gameObject);
            }
        }
    }
}
