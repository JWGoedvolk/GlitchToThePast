using GlitchInThePast.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerRangedProjectile prefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<PlayerRangedProjectile> pool = new Queue<PlayerRangedProjectile>();
    private Transform container;
    #endregion

    private void Awake()
    {
        container = transform;
        SetupProjectilePool();
    }

    #region Public Functions
    public PlayerRangedProjectile Spawn(Vector3 position, Quaternion rotation)
    {
        PlayerRangedProjectile proj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, container);
        if (proj.gameObject.activeSelf) proj.gameObject.SetActive(false); // safety
        proj.transform.SetPositionAndRotation(position, rotation);
        proj.SetPool(this);
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void Despawn(PlayerRangedProjectile proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
    #endregion

    #region Private Functions
    private void SetupProjectilePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var projectile = Instantiate(prefab, container);
            projectile.gameObject.SetActive(false);
            projectile.SetPool(this);
            pool.Enqueue(projectile);
        }
    }
    #endregion
}