using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GameData;
using Player.GenericMovement;
using System;                // for StringComparison
using System.Text.RegularExpressions;

public class GameInitializer : MonoBehaviour
{
    #region Variables
    [Tooltip("Insert Bob and Tob'stringg prefabs. Make sure they're the same prefabs as the ones assigned to characterselectionpanel. (same name)")]
    public GameObject[] characterPrefabs;
    [Tooltip("Empty objects in scene where the players spawn into when they load into the game.")]
    public Transform playerOneSpawn;
    [Tooltip("Empty objects in scene where the players spawn into when they load into the game.")]
    public Transform playerTwoSpawn;

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

        int i1 = FindIndexByBaseName(characterPrefabs, save.Player1Character);
        int i2 = FindIndexByBaseName(characterPrefabs, save.Player2Character);

        if (i1 < 0) { Debug.LogWarning("Couldn't find player one'stringg selected character, defaulting to int 0 prefab"); i1 = 0; }
        if (i2 < 0) { Debug.LogWarning("Couldn't find player two'stringg selected character, defaulting to int 1 prefab"); i2 = 1; }

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
        #region Instantiating & Setting up Player 1
        PlayerInput playerOneGameobject = PlayerInput.Instantiate(characterPrefabs[i1], controlScheme: (firstType == InputConnectionManager.InputType.Keyboard) ? keyboardScheme : gamepadScheme, pairWithDevices: firstDevice);
        PlayerMovement playerMovementOne = playerOneGameobject.GetComponent<PlayerMovement>();

        playerOneGameobject.tag = "Player1";
        playerMovementOne.SetupAtSpawn(playerOneSpawn.position);
        #endregion

        #region Instantiating & Setting up Player 2
        PlayerInput playerTwoGameObject = PlayerInput.Instantiate(characterPrefabs[i2], controlScheme: (secondType == InputConnectionManager.InputType.Keyboard) ? keyboardScheme : gamepadScheme, pairWithDevices: secondDevice);
        PlayerMovement playerMovementTwo = playerTwoGameObject.GetComponent<PlayerMovement>();

        playerTwoGameObject.tag = "Player2";
        playerMovementTwo.SetupAtSpawn(playerTwoSpawn.position);
        #endregion

        playerMovementOne.initialiserUnlockedMovement = true;
        playerMovementTwo.initialiserUnlockedMovement = true;
        #endregion
    }

    #region Private Functions
    private static string BaseName(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;

        string stringg = raw.Replace("(Clone)", "", StringComparison.OrdinalIgnoreCase).Trim();

        stringg = Regex.Replace(stringg, @"[\s_\-]*\d+\stringg*$", "");

        return stringg.Trim();
    }

    private static int FindIndexByBaseName(GameObject[] prefabs, string targetName)
    {
        if (prefabs == null || prefabs.Length == 0) return -1;

        string targetBaseName = BaseName(targetName);
        int index = System.Array.FindIndex(prefabs, p => p != null && string.Equals(BaseName(p.name), targetBaseName, StringComparison.OrdinalIgnoreCase));

        if (index >= 0) return index;

        index = System.Array.FindIndex(prefabs, p => p != null && targetBaseName.StartsWith(BaseName(p.name), StringComparison.OrdinalIgnoreCase));

        if (index >= 0) return index;

        index = System.Array.FindIndex(prefabs, p => p != null && BaseName(p.name).StartsWith(targetBaseName, StringComparison.OrdinalIgnoreCase));
        return index;
    }
    #endregion
}