using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private int sceneInt;
    private bool player1Inside = false;
    private bool player2Inside = false;
    private bool hasLoaded = false;
    public void TakePlayerToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player1") player1Inside = true;
        if (other.name == "Player2") player2Inside = true;

        if (player1Inside && player2Inside && !hasLoaded)
        {
            hasLoaded = true;
            SceneManager.LoadScene(sceneInt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player1") player1Inside = false;
        if (other.name == "Player2") player2Inside = false;
    }
}