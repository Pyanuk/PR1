using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponEnum WeaponType;

    public int MaxMagazineBulletCount;
    public int CurrentMagazineBulletCount;
    public int MaxAmmoSupply;
    public float TimeBetweenShots;
    public int TimeForReloading;

    bool CanFire = true;
    bool IsReloading = false;

    public AudioSource ShotSound;
    public AudioSource ReloadSound;

    public bool isUnlocked;

    public IEnumerator LockFire(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanFire = true;
    }

    public IEnumerator LockFireForReloadi(float Time)
    {
        ReloadSound.Play();
        yield return new WaitForSeconds(Time);
        CurrentMagazineBulletCount = MaxMagazineBulletCount;
        CanFire = true;
        IsReloading = false;
        Debug.Log("Перезарядка завершена!");
    }

    public bool IsMagazineEmpty()
    {
        return CurrentMagazineBulletCount == 0;
    }

    public void Reload()
    {
        if (!IsReloading)
        {
            Debug.Log("Перезарядка");
            IsReloading = true;
            CanFire = false;
            StartCoroutine(LockFireForReloadi(TimeForReloading));
        }
    }

    public void Fire()
    {
        if (CanFire && !IsMagazineEmpty() && !IsReloading)
        {
            Debug.Log(CurrentMagazineBulletCount);
            ShotSound.Play();
            CurrentMagazineBulletCount--;

            RaycastHit HitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo))
            {
                Debug.Log(HitInfo.transform.name);
            }

            CanFire = false;
            StartCoroutine(LockFire(TimeBetweenShots));
        }
    }
}
