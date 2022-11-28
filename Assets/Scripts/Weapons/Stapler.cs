using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stapler : BaseWeapon
{
    [Header("Stapler")]
    [SerializeField]
    private FMODUnity.EventReference staplerImpactSoundEvent;

    private FMOD.Studio.EventInstance staplerImpactSoundInstance;

    protected override void OnHit()
    {
        base.OnHit();
        SoundUtils.PlaySound3D(ref staplerImpactSoundInstance, staplerImpactSoundEvent, gameObject);
    }
}
