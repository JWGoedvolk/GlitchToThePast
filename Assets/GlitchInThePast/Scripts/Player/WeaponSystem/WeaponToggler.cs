using JW.Roguelike.Objects;
using UnityEngine;

namespace GlitchInThePast.Scripts.Player
{
    public class WeaponToggler : CustomTriggerer
    {
        [SerializeField] private bool isEnabler = false;

        public override void OnTrigger(GameObject other)
        {
            PlayerWeaponSystem weaponSystem = other.GetComponent<PlayerWeaponSystem>();
            if (weaponSystem != null)
            {
                if (isEnabler)
                {
                    weaponSystem.EnableWeapon();
                }
                else
                {
                    weaponSystem.DisableWeapon();
                }
            }
        }
    }
}