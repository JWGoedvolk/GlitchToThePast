using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZLayerSorter : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isNotMoving;
    #endregion

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100);
    }

    void LateUpdate()
    {
        if (isNotMoving) return;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100);
    }
}