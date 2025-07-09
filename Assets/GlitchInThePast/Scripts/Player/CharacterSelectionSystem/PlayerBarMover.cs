using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBarMover : MonoBehaviour
{
    public enum Player { P1, P2 }

    #region Variables
    [Tooltip("Which player this bar belongs to")]
    public Player player;

    [Tooltip("0 = Left slot, 1 = Center slot, 2 = Right slot")]
    public RectTransform[] slots;

    public float glideSpeed = 10f;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 10f;

    private RectTransform barRt;
    private Vector2 targetAnchoredPos;
    private int currentIndex, lastDir;

    // made it static so that p1 and p2 know where eachothers' positions are
    public static int p1Index = 1, p2Index = 1;
    public static bool P1Locked = false, P2Locked = false;
    #endregion

    void Awake()
    {
        barRt = GetComponent<RectTransform>();
        currentIndex = (player == Player.P1 ? p1Index : p2Index);
        targetAnchoredPos = slots[currentIndex].anchoredPosition;
        barRt.anchoredPosition = targetAnchoredPos;
    }

    void Update()
    {
        barRt.anchoredPosition = Vector2.Lerp(barRt.anchoredPosition, targetAnchoredPos, Time.deltaTime * glideSpeed);
    }

    #region Movement Functions
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if ((player == Player.P1 && P1Locked) ||
            (player == Player.P2 && P2Locked))
            return;

        float x = ctx.ReadValue<Vector2>().x;
        int dir = (x < -0.5f ? -1 : x > 0.5f ? 1 : 0);

        if (dir != 0 && dir != lastDir)
        {
            int desired = Mathf.Clamp(currentIndex + dir, 0, slots.Length - 1);

            if (desired == currentIndex)
                desired = 1;

            if (desired != 1)
            {
                bool blocked =
                    (player == Player.P1 && p2Index == desired) ||
                    (player == Player.P2 && p1Index == desired);

                if (blocked)
                {
                    StartCoroutine(Shake());
                    lastDir = dir;
                    return;
                }
            }

            currentIndex = desired;
            if (player == Player.P1) p1Index = desired; else p2Index = desired;
            targetAnchoredPos = slots[desired].anchoredPosition;
        }

        lastDir = dir;
    }

    private IEnumerator Shake()
    {
        Vector2 original = barRt.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            var offset = Random.insideUnitCircle * shakeMagnitude;
            barRt.anchoredPosition = original + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }
        barRt.anchoredPosition = original;
    }
    #endregion
}