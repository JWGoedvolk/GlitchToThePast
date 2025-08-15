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
        public MeleeSpawner MeleeSpawner;
        public RangedSpawner RangedSpawner;
        [Header("Spawner")] 
        public List<Transform> MeleeSpawnPoints; // index 0 = melee player is on the left
        public List<Transform> RangedSpawnPoints; // index 0 = ranged player is on the right
        public List<SpriteRenderer> SpawnerSprites;

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
                MeleeSpawner.SpawnPoint  = MeleeSpawnPoints[0];
                RangedSpawner.SpawnPoint = RangedSpawnPoints[0];
                
                // Activate all the used spawner sprites
                SpawnerSprites[0].enabled = true;
                SpawnerSprites[1].enabled = false;
                SpawnerSprites[2].enabled = true;
                SpawnerSprites[3].enabled = false;
                
                // Turn off all unused MeleeSpawner points
                MeleeSpawnPoints [1].gameObject.SetActive(false);
                RangedSpawnPoints[1].gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Player 2 is melee");
                // Set the second set of spawnpoints the used ones
                MeleeSpawner.SpawnPoint  = MeleeSpawnPoints[1];
                RangedSpawner.SpawnPoint = RangedSpawnPoints[1];
                
                // Activate all the used spawner sprites
                SpawnerSprites[0].enabled = false;
                SpawnerSprites[1].enabled = true;
                SpawnerSprites[2].enabled = false;
                SpawnerSprites[3].enabled = true;
                
                MeleeSpawnPoints [0].gameObject.SetActive(false);
                RangedSpawnPoints[0].gameObject.SetActive(false);
            }
        }
    }
}