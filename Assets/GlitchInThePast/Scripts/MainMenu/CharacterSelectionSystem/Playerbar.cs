using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBarMover : MonoBehaviour
{
    public enum Player { P1, P2 }
    #region Variables
    public Player player;

    public RectTransform leftTarget, rightTarget, originalPosition;

    RectTransform barRT;
    Vector2 targetAnchoredPos;

    public static RectTransform P1Target, P2Target;
    #endregion

    private void Awake()
    {
        barRT = GetComponent<RectTransform>();
        targetAnchoredPos = originalPosition.anchoredPosition;

        if (player == Player.P1) P1Target = originalPosition;
        else P2Target = originalPosition;
    }

    private void Update()
    {
        barRT.anchoredPosition = Vector2.Lerp(barRT.anchoredPosition, targetAnchoredPos, Time.deltaTime * 10f);
    }

    #region Public Functions
    public void OnMove(InputAction.CallbackContext ctx)
    {
        float x = ctx.ReadValue<Vector2>().x;

        bool onLeft = Vector2.Distance(targetAnchoredPos, leftTarget.anchoredPosition) < 1f;
        bool onRight = Vector2.Distance(targetAnchoredPos, rightTarget.anchoredPosition) < 1f;

        if ((onLeft && x < -0.5f) ||
            (onRight && x > 0.5f))
        {
            SetTarget(originalPosition);
            return;
        }

        if (x < -0.5f)
            TryMoveTo(leftTarget);
        else if (x > 0.5f)
            TryMoveTo(rightTarget);
    }
    #endregion

    private void TryMoveTo(RectTransform newT)
    {
        bool blocked = (player == Player.P1 && P2Target == newT)
                    || (player == Player.P2 && P1Target == newT);

        if (!blocked)
            SetTarget(newT);
    }

    private void SetTarget(RectTransform t)
    {
        targetAnchoredPos = t.anchoredPosition;
        if (player == Player.P1) P1Target = t; else P2Target = t;
    }
}