using System;
using JW.Roguelike.Objects;
using UnityEngine;

namespace Systems.Enemies.Boss
{
    public class Shockwave : CustomTriggerer
    {
        [Header("Shockwave Expansion")] 
        [SerializeField] private float startingRange = 0.5f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float duration = 2f;
        [SerializeField] private float currentTime = 0f;

        private void OnEnable()
        {
            transform.localScale = new Vector3(startingRange, startingRange, startingRange);
        }

        private void Update()
        {
            // Grow the shockwave to the intended range over the given duration
            currentTime += Time.deltaTime;
            float scale = Mathf.Lerp(startingRange, range, currentTime / duration);
            transform.localScale = new Vector3(scale, scale, scale);

            // Destroy the shockwave game object when it has reached its final size
            if (currentTime >= duration)
            {
                Destroy(this.gameObject);
            }
        }

        public override void OnTrigger(GameObject other)
        {
            PlayerHealthSystem playerHealthSystem = other.GetComponent<PlayerHealthSystem>();
            if (playerHealthSystem != null)
            {
                playerHealthSystem.TakeDamage(1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range/2);
        }
    }
}