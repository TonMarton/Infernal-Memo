using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Shotgun : BaseWeapon
{
    protected override bool UseAmmoInClip()
    {
        return weaponShared.playerStats.UseShellsInClip(shellsShootCost);
    }
}