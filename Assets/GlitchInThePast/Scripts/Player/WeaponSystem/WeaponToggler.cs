using JW.BeatEmUp.Objects;
using UnityEngine;

namespace GlitchInThePast.Scripts.Player
{
    public class WeaponToggler : CustomTriggerer
    {
        [SerializeField] private bool isEnabler = false;
        public PlayerWeaponSystem.WeaponType TargetWeaponType = PlayerWeaponSystem.WeaponType.None;

        public override void OnTrigger(GameObject other)
        {
            foreach (GameObject triggeringObject in TriggeringObjects)
            {
                PlayerWeaponSystem weaponSystem = triggeringObject.GetComponentInChildren<PlayerWeaponSystem>();
                if (weaponSystem != null)
                {
                    if (isEnabler)
                    {
                        if (weaponSystem.Weapon == TargetWeaponType || TargetWeaponType == PlayerWeaponSystem.WeaponType.None)
                        {
                            weaponSystem.EnableWeapon();
                            onTrigger?.Invoke();
                        }
                    }
                    else
                    {
                        if (weaponSystem.Weapon == TargetWeaponType || TargetWeaponType == PlayerWeaponSystem.WeaponType.None)
                        {
                            weaponSystem.DisableWeapon();
                            onTrigger?.Invoke();
                        }
                    }
                }
            }
        }
    }
}