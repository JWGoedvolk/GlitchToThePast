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

    [Tooltip("How fast the bar glides to its new position")]
    public float glideSpeed = 10f;

    private int currentIndex;
    private RectTransform barRt;
    private int lastDir = 0;
    private Vector2 targetAnchoredPos;

    // made it static so that p1 and p2 know where eachothers' positions are
    public static int p1Index = 1;
    public static int p2Index = 1;
    #endregion

    void Awake()
    {
        barRt = GetComponent<RectTransform>();

        if (player == Player.P1) p1Index = 1;
        else p2Index = 1;

        currentIndex = (player == Player.P1) ? p1Index : p2Index;
        targetAnchoredPos = slots[currentIndex].anchoredPosition;
        barRt.anchoredPosition = targetAnchoredPos;
    }

    void Update()
    {
        barRt.anchoredPosition = Vector2.Lerp(barRt.anchoredPosition, targetAnchoredPos, Time.deltaTime * glideSpeed);
    }

    #region Movement Function
    public void OnMove(InputAction.CallbackContext ctx)
    {
        float x = ctx.ReadValue<Vector2>().x;

        int direction = 0;
        if (x < -0.5f) direction = -1;
        else if (x > 0.5f) direction = 1;

        if (direction != 0 && direction != lastDir)
        {
            int desired = Mathf.Clamp(currentIndex + direction, 0, slots.Length - 1);

            if (desired == currentIndex)
                desired = 1;

            if (desired != 1)
            {
                if (player == Player.P1 && p2Index == desired)
                {
                    lastDir = direction;
                    return;
                }
                if (player == Player.P2 && p1Index == desired)
                {
                    lastDir = direction;
                    return;
                }
            }

            currentIndex = desired;
            if (player == Player.P1) p1Index = currentIndex;
            else p2Index = currentIndex;

            targetAnchoredPos = slots[currentIndex].anchoredPosition;
        }

        lastDir = direction;
    }

    #endregion
}
