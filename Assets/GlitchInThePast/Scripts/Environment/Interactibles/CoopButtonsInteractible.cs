using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JW.BeatEmUp.Objects;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Objects.Interactibles
{
    public class CoopButtonsInteractible : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private List<CustomTriggerer> buttons;

        [Header("Events")]
        public bool AllPressedInvoked = false;
        [SerializeField] private UnityEvent onAllButtonsPressed;
        public bool AllReleasedInvoked = false;
        [SerializeField] private UnityEvent onAllButtonsReleased;

        void Update()
        {
            List<CustomTriggerer> triggeredButtons = buttons.Where(button => button.IsTriggering).ToList(); // Get all the buttons which are being triggered
            if (triggeredButtons.Count == buttons.Count) // If the number of buttons being triggered is the same as the number of buttons, they are all pressed
            {
                if (!AllPressedInvoked) // The event has not been invoked yet
                {
                    onAllButtonsPressed?.Invoke();
                    AllPressedInvoked = true; // Stops us from invoking the event every frame
                    AllReleasedInvoked = false;
                }
            }
            else if (AllPressedInvoked && triggeredButtons.Count < buttons.Count) // All the buttons were recently pressed but now aren't anymore
            {
                if (!AllReleasedInvoked)
                {
                    onAllButtonsReleased?.Invoke();
                    AllReleasedInvoked = true;
                    AllPressedInvoked = false;
                }
            }
        }
    }
}