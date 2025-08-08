using System;
using System.Collections.Generic;
using GlitchInThePast.Scripts.Player;
using Systems.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Environment
{
    public class Section3SetUp : MonoBehaviour
    {
        public EnemySpawner Spawner;
        [Header("Spawner")] 
        public List<Transform> MeleeSpawnPoints; // index 0 = melee player is on the left
        public List<Transform> RangedSpawnPoints; // index 0 = ranged player is on the right

        private void Start()
        {
            SetUpSpawnPoints();
        }

        public void SetUpSpawnPoints()
        {
            if (PlayerInput.all[0].GetComponent<PlayerWeaponSystem>().Weapon == PlayerWeaponSystem.WeaponType.Melee)
            {
                Debug.Log("Player 1 is melee");
                // Set the first set of spawnpoints the used ones
                Spawner.meleeSpawnPoint  = MeleeSpawnPoints[0];
                Spawner.rangedSpawnPoint = RangedSpawnPoints[0];
                
                // Turn off all unused spawner points
                MeleeSpawnPoints [1].gameObject.SetActive(false);
                RangedSpawnPoints[1].gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Player 2 is melee");
                // Set the second set of spawnpoints the used ones
                Spawner.meleeSpawnPoint  = MeleeSpawnPoints[1];
                Spawner.rangedSpawnPoint = RangedSpawnPoints[1];
                
                MeleeSpawnPoints [0].gameObject.SetActive(false);
                RangedSpawnPoints[0].gameObject.SetActive(false);
            }
        }
    }
}