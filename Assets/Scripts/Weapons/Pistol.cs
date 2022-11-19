using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BaseWeapon
{
    protected override bool UseAmmoInClip()
    {
        return weaponShared.playerStats.UseBulletsInClip(shellsShootCost);
    }
}
