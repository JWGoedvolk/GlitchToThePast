using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Objects.Interactibles
{
    public class CoopButtonsInteractible : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private CustomTriggerer button1;
        [SerializeField] private CustomTriggerer button2;

        [Header("Events")]
        [SerializeField] private UnityEvent onBothButtonsPressed;
        [SerializeField] private UnityEvent onBothButtonsReleased;
        [SerializeField][ReadOnly] private bool isBothButtonsPressedTriggered = false;

        void Update()
        {
            if (button1.IsTriggering && button2.IsTriggering && !isBothButtonsPressedTriggered)
            {
                onBothButtonsPressed.Invoke();
                isBothButtonsPressedTriggered = true;
            }

            // Check if we have both buttons triggered and step off one of them.
            // NOTE: ^ is the XOR opperand, it will only return true if one of its inputs are true ie. only one buttons is being triggered
            if (isBothButtonsPressedTriggered && (button1.IsTriggering ^ button2.IsTriggering))
            {
                onBothButtonsReleased.Invoke();
                isBothButtonsPressedTriggered = false;
            }
        }
    }
}