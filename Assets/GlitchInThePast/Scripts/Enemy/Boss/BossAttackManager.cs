using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Enemies.Boss
{
    public class BossAttackManager : MonoBehaviour
    {
        [SerializeField][Range(0f, 5f)] private float attackTimeVariance;
        [SerializeField] private float baseAttackFrequency = 7f;
        float currentTime = 0f;
        private float nextAttackTime;
        private Animator animator;

        void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= nextAttackTime)
            {
                currentTime = 0f;
                animator.SetTrigger("Attack");

                nextAttackTime = baseAttackFrequency + Random.Range(-attackTimeVariance, attackTimeVariance);
                Debug.Log($"Next attack in {nextAttackTime}");
            }
        }
    }
}