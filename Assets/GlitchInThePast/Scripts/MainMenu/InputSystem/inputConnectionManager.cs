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
    #endregion

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        AssignInputs();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad && (change == InputDeviceChange.Added || change == InputDeviceChange.Removed) && characterSelectionPanel.activeInHierarchy)
        {
            AssignInputs();
        }
    }

    public void AssignInputs()
    {
        var controllers = Gamepad.all.Where(g => g.added && g.description.interfaceName == "XInput").Take(2).ToList();
        var user1 = player1Input.user;
        var user2 = player2Input.user;

        foreach (var d in user1.pairedDevices.ToArray())user1.UnpairDevice(d);
        foreach (var d in user2.pairedDevices.ToArray())user2.UnpairDevice(d);
            
        if (controllers.Count == 0)
        {
            Player1InputType = InputType.Keyboard;
            Player2InputType = InputType.None;

            player1Input.SwitchCurrentControlScheme("Keyboard", Keyboard.current, Mouse.current);
            Debug.Log("P1=Keyboard, P2=None");
        }

        else if (controllers.Count == 1)
        {
            Player1InputType = InputType.Keyboard;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Keyboard", Keyboard.current, Mouse.current);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);

            Debug.Log($"P1=Keyboard, P2={controllers[0].description.product}");
        }

        else
        {
            Player1InputType = InputType.Controller2;
            Player2InputType = InputType.Controller1;

            player1Input.SwitchCurrentControlScheme("Controller", controllers[1]);
            player2Input.SwitchCurrentControlScheme("Controller", controllers[0]);

            Debug.Log($"P1={controllers[1].description.product}, P2={controllers[0].description.product}");
        }
    }
}