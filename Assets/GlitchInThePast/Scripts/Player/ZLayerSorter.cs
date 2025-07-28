using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZLayerSorter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100);
    }
}