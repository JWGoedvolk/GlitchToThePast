using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnectionManager : MonoBehaviour
{
    public enum InputType { None, Keyboard, Controller1, Controller2 }
    [HideInInspector] public static InputType Player1InputType, Player2InputType;

    public PlayerInput player1Input, player2Input;
    public GameObject characterSelectionPanel;

    bool hasAssigned = false;

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        hasAssigned = false;
        TryAssignOnPanelOpen();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!hasAssigned && characterSelectionPanel.activeInHierarchy
            && device is Gamepad
            && (change == InputDeviceChange.Added || change == InputDeviceChange.Removed))
        {
            AssignInputs();
        }
    }

    void TryAssignOnPanelOpen()
    {
        if (characterSelectionPanel.activeInHierarchy && !hasAssigned)
            AssignInputs();
    }

    public void AssignInputs()
    {
        var controllers = Gamepad.all.Where(g => g.added)  .GroupBy(g => g.description.product) .Select(grp => grp.First()) .ToList();

        Debug.Log($"Connected Devices: {string.Join(", ", controllers.Select(g => g.description.product))}");

        if (controllers.Count == 0)
        {
            Player1InputType = Player2InputType = InputType.None;
            Debug.LogError("You need at least one controller!"); //TODO: Reflect this through UI.
            return;
        }

        if (controllers.Count == 1)
        {
            Player1InputType = InputType.Keyboard;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);
        }
        else if(controllers.Count == 2)
        {
            Player1InputType = InputType.Controller2;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);
        }

        hasAssigned = true;
        Debug.Log($"[InputMgr] P1: {Player1InputType}, P2: {Player2InputType}");
    }
}