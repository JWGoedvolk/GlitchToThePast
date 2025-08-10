using System.Collections.Generic;
using Systems.Enemies;
using UnityEngine;

namespace GlitchInThePast.Scripts.Player
{
    public class AttackCaster : MonoBehaviour
    {
        public Bounds AttackArea;
        public List<EnemyHealth> EnemiesInArea;

        public void CastAttack()
        {
            EnemiesInArea = new List<EnemyHealth>();
            Collider[] results = new Collider[] { };
            var hits = Physics.OverlapBoxNonAlloc(AttackArea.center, AttackArea.extents, results);
            foreach (Collider result in results)
            {
                if (result.transform.tag == "Enemy")
                {
                    EnemyHealth eh = result.GetComponent<EnemyHealth>();
                    EnemiesInArea.Add(eh);
                }
            }
        }
    }
}