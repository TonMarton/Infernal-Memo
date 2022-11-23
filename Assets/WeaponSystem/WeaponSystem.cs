using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI weaponText;


    [System.Serializable]
    public struct StateParams
    {
        public float duration;
        public static readonly StateParams Default = new StateParams { duration = 1.0f };
    }

    [System.Serializable]
    public class WeaponParams
    {
        public Image weaponImage;
        public Sprite defaultSprite;
        public Sprite fireSprite;
        public float fireSpriteNormalizedDuration = 0.25f;
        public StateParams raise = StateParams.Default;
        public StateParams idle = StateParams.Default;
        public StateParams attack = StateParams.Default;
        public StateParams reload = StateParams.Default;
        public StateParams lower = StateParams.Default;

        public StateParams GetParams(State value)
        {
            switch (value)
            {
                case State.None:
                    return default;
                case State.Raise:
                    return raise;
                case State.Idle:
                    return idle;
                case State.Attack:
                    return attack;
                case State.Reload:
                    return reload;
                case State.Lower:
                    return lower;
                default:
                    return default;
            }
        }
    }

    public WeaponParams stapler;
    public WeaponParams pistol;
    public WeaponParams shotgun;

    public WeaponParams[] weapons;


    public enum State
    {
        None,
        Raise,
        Idle,
        Attack,
        Reload,
        Lower
    }

    public enum Weapon
    {
        None,
        Stapler,
        Pistol,
        Shotgun
    }

    State currentState = State.None;
    Weapon currentWeapon = Weapon.None;
    Weapon desiredWeapon = Weapon.None;
    float stateTime;


    public WeaponParams GetWeaponParams(Weapon value)
    {
        switch (value)
        {
            case Weapon.Stapler:
                return stapler;
            case Weapon.Pistol:
                return pistol;
            case Weapon.Shotgun:
                return shotgun;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
    private void Awake()
    {
        weapons = new WeaponParams[] { stapler, pistol, shotgun };
    }
    IEnumerator Start()
    {
        SetWeapon(Weapon.None);
        yield return new WaitForSeconds(0.5f);
        SetWeapon(Weapon.Stapler);
        desiredWeapon = currentWeapon;
    }

    void SetWeapon(Weapon value)
    {
        currentWeapon = value;
        if (value == Weapon.None)
        {
            SetState(State.None, 0);
        }
        else
        {
            SetState(State.Raise, 0);
        }
        for (int i = 0; i < 3; i++)
        {
            Weapon weapon = (Weapon)(i + 1);
            weapons[i].weaponImage.gameObject.SetActive(weapon == value);
        }
    }

    void SetState(State value, float time)
    {
        currentState = value;
        stateTime = time;
        if (value != State.None)
        {
            OnStateUpdate();

        }
    }

    //void BeginSwitchWeapon(Weapon newWeapon)
    //{
    //    SetState(State.Lower);
    //    desiredWeapon = newWeapon;
    //}
    void OnStateUpdate()
    {
        var weap = GetWeaponParams(currentWeapon);
        var img = weap.weaponImage;
        var rt = img.rectTransform;
        float yOffset = 0;
        float height = rt.rect.height;
        Sprite sprite = weap.defaultSprite;
        // state update
        switch (currentState)
        {
            case State.Raise:
                yOffset = -height * (1 - stateTime);
                if (desiredWeapon != currentWeapon)
                {
                    float lastStateTime = stateTime;
                    SetState(State.Lower, 0);
                    stateTime = 1 - lastStateTime;
                }
                break;
            case State.Idle:
                if (desiredWeapon != currentWeapon)
                {
                    SetState(State.Lower, 0);
                }
                else if (Input.GetMouseButton(0))
                {
                    SetState(State.Attack, 0);
                }
                break;
            case State.Attack:
                if (stateTime < weap.fireSpriteNormalizedDuration)
                {
                    sprite = weap.fireSprite;
                }
                break;
            case State.Reload:
                break;
            case State.Lower:
                yOffset = -height * stateTime;

                break;
        }
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yOffset);
        img.sprite = sprite;
    }
    void OnStateEnd()
    {
        // state end
        switch (currentState)
        {
            case State.Raise:
                SetState(State.Idle, stateTime);
                break;
            case State.Idle:
                break;
            case State.Attack:
                if (Input.GetMouseButton(0) && desiredWeapon == currentWeapon)
                {
                    SetState(State.Attack, stateTime);
                }
                else
                {
                    SetState(State.Idle, stateTime);
                }
                break;
            case State.Reload:
                break;
            case State.Lower:
                SetWeapon(desiredWeapon);
                break;
        }
    }
    private void LateUpdate()
    {
        stateText.text = currentState.ToString();
        weaponText.text = currentWeapon.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            desiredWeapon = Weapon.Stapler;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            desiredWeapon = Weapon.Pistol;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            desiredWeapon = Weapon.Shotgun;
        }

        if (currentState != State.None)
        {
            stateTime += Time.deltaTime / GetWeaponParams(currentWeapon).GetParams(currentState).duration;
            if (stateTime >= 1f)
            {
                stateTime %= 1f;
                OnStateEnd();
            }
            else
            {
                OnStateUpdate();
            }
        }



    }
}
