using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GameData;

public class GameInitializer : MonoBehaviour
{
    #region Variables
    [Tooltip("Insert Bob and Tob's prefabs. Make sure they're the same prefabs as the ones assigned to characterselectionpanel. (same name)")]
    public GameObject[] characterPrefabs;
    [Tooltip("Empty objects in scene where the players spawn into when they load into the game.")]
    public Transform playerOneSpawn;
    [Tooltip("Empty objects in scene where the players spawn into when they load into the game.")]
    public Transform player2Spawn;

    private string keyboardScheme = "Keyboard";
    private string gamepadScheme = "Controller";
    #endregion

    void Awake()
    {
        GameSaveData save = GameSaveSystem.LoadGame();
        if (save == null)
        {
            Debug.LogError("Did not find a saved file. Failed to run the game.");
            SceneManager.LoadScene(0);
            return;
        }

        if (characterPrefabs == null || characterPrefabs.Length < 2)
        {
            Debug.LogError("Make sure there are two character prefabs assigned.");
            return;
        }

        int i1 = System.Array.FindIndex(characterPrefabs, prefab => prefab != null && prefab.name == save.Player1Character);
        int i2 = System.Array.FindIndex(characterPrefabs, prefab => prefab != null && prefab.name == save.Player2Character);

        if (i1 < 0) { Debug.LogWarning("Couldn't find player one's selected character, defaulting to int 0 prefab"); i1 = 0;}
        if (i2 < 0) { Debug.LogWarning("Couldn't find player two's selected character, defaulting to int 1 prefab"); i2 = 1;}

        InputConnectionManager.InputType firstType, secondType;
        if (!System.Enum.TryParse(save.Player1Input, out firstType)) firstType = InputConnectionManager.InputType.Keyboard;
        if (!System.Enum.TryParse(save.Player2Input, out secondType)) secondType = InputConnectionManager.InputType.Controller1;

        var pads = Gamepad.all.Where(g => g.added && g.description.interfaceName == "XInput").Take(2).ToList();

        InputDevice[] firstDevice, secondDevice;

        if (firstType == InputConnectionManager.InputType.Keyboard || pads.Count == 0) firstDevice = new[] { Keyboard.current };
        else
        {
            Gamepad gamepad = pads.Count > 1 ? pads[1] : pads[0];
            firstDevice = new[] { gamepad };
        }

        if (secondType == InputConnectionManager.InputType.Keyboard || pads.Count == 0) secondDevice = new[] { Keyboard.current };
        else
        {
            Gamepad gamepad = pads[0];
            secondDevice = new[] { gamepad };
        }

        #region Players' instantiation
        PlayerInput playerOneGameobject = PlayerInput.Instantiate(characterPrefabs[i1], controlScheme: (firstType == InputConnectionManager.InputType.Keyboard) ? keyboardScheme : gamepadScheme, pairWithDevices: firstDevice);
        playerOneGameobject.transform.position = playerOneSpawn.position;

        PlayerInput playerTwoGameObject = PlayerInput.Instantiate(characterPrefabs[i2], controlScheme: (secondType == InputConnectionManager.InputType.Keyboard) ? keyboardScheme : gamepadScheme, pairWithDevices: secondDevice);
        playerTwoGameObject.transform.position = player2Spawn.position;
        #endregion
    }
}
