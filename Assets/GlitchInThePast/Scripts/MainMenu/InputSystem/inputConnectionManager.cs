using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputConnectionManager : MonoBehaviour
{
    public enum InputType { None, Keyboard, Controller1, Controller2 }
    [HideInInspector] public static InputType Player1InputType, Player2InputType;

    #region Variables
    public PlayerInput player1Input, player2Input;
    public GameObject characterSelectionPanel;

    bool hasAssigned = false;
    #endregion

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        hasAssigned = false;
        TryAssignOnPanelOpen();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!hasAssigned && characterSelectionPanel.activeInHierarchy
            && device is Gamepad
            && (change == InputDeviceChange.Added || change == InputDeviceChange.Removed))
        {
            AssignInputs();
        }
    }

    private void TryAssignOnPanelOpen()
    {
        if (characterSelectionPanel.activeInHierarchy && !hasAssigned)
            AssignInputs();
    }

    public void AssignInputs()
    {
        var controllers = Gamepad.all
        .Where(g => g.added && g.description.interfaceName == "XInput")
        .Take(2)
        .ToList();

        Debug.Log($"Found {controllers.Count} XInput pads: " + $"{string.Join(", ", controllers.Select(c => c.description.product))}");
                  
        if (controllers.Count == 0)
        {
            Debug.LogError("No gamepads found!");
            return;
        }

        if (controllers.Count <= 1)
        {
            Player1InputType = InputType.Keyboard;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Keyboard", Keyboard.current, Mouse.current);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);
        }
        else
        {
            Player1InputType = InputType.Controller2;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Controller", controllers[1]);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);
        }

        hasAssigned = true;
        Debug.Log($"P1: {Player1InputType}, P2: {Player2InputType}");
    }
}